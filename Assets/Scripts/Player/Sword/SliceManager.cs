using UnityEngine;
using EzySlice;
using System.Collections;

public class SliceManager : MonoBehaviour
{
    public Material slicedMaterial;
    public Transform sword;

    private const float SliceForce = 5;
    private const float DestructionDelay = 1f;
    private const float BalloonMass = 0.1f;
    private const float BalloonDrag = 5f;
    private const float BalloonAngularDrag = 1f;


    public void SliceObject(GameObject slicedGameObject, Collision collision)
    {
        if (!CanSlice(slicedGameObject))
        {
            Debug.LogWarning("slicedGameObject does not have a MeshFilter or MeshCollider. Slicing will fail.");
            return;
        }

        Vector3 slicePosition = collision.contacts[0].point;
        Vector3 swordDirection = sword.forward;

        SlicedHull slicedObject = Slice(slicedGameObject, slicePosition, swordDirection);

        if (slicedObject == null)
        {
            Debug.LogWarning($"Slicing failed for {slicedGameObject.name}. Using default slicing plane through the midpoint.");
            HandleMidpointSlice(slicedGameObject);
        }
        else
        {
            HandleSlicedObject(slicedGameObject, slicedObject, slicePosition, swordDirection);
        }
    }

    private void HandleMidpointSlice(GameObject slicedGameObject)
    {
        Bounds objectBounds = slicedGameObject.GetComponent<Renderer>().bounds;
        Vector3 midpoint = objectBounds.center;

        // Slice through the midpoint with a default vertical slicing plane
        Vector3 defaultSliceDirection = Vector3.up; // Clean vertical slice
        SlicedHull slicedObject = Slice(slicedGameObject, midpoint, defaultSliceDirection);

        if (slicedObject == null)
        {
            Debug.LogError("Midpoint slicing also failed. Check the object's mesh and slicing parameters.");
            return;
        }

        HandleSlicedObject(slicedGameObject, slicedObject, midpoint, defaultSliceDirection);
    }


    private bool CanSlice(GameObject slicedGameObject)
    {
        return slicedGameObject.GetComponent<MeshFilter>() != null && slicedGameObject.GetComponent<MeshCollider>() != null;
    }

    private SlicedHull Slice(GameObject target, Vector3 position, Vector3 direction)
    {
        return target.Slice(position, direction, slicedMaterial);
    }

    private void HandleSlicedObject(GameObject originalObject, SlicedHull slicedObject, Vector3 slicePosition, Vector3 swordDirection)
    {
        Destroy(originalObject);
        GameObject upperHull = CreateHull(slicedObject.CreateUpperHull(originalObject, slicedMaterial));
        GameObject lowerHull = CreateHull(slicedObject.CreateLowerHull(originalObject, slicedMaterial));

        ApplyForces(upperHull, lowerHull, slicePosition, swordDirection);
        StartCoroutine(ScheduleDestruction(upperHull));
        StartCoroutine(ScheduleDestruction(lowerHull));
    }

    private GameObject CreateHull(GameObject hull)
    {
        if (hull == null) return null;

        Rigidbody rb = hull.AddComponent<Rigidbody>();
        BoxCollider collider = hull.AddComponent<BoxCollider>();

        rb.mass = BalloonMass;
        rb.linearDamping = BalloonDrag;
        rb.angularDamping = BalloonAngularDrag;
        rb.useGravity = true;

        collider.isTrigger = false; // Allow interaction but not blocking

        hull.transform.position = transform.position;
        hull.transform.rotation = transform.rotation;
        hull.transform.localScale = transform.localScale;

        hull.layer = 10;
        return hull;
    }

    private void ApplyForces(GameObject upperHull, GameObject lowerHull, Vector3 slicePosition, Vector3 swordDirection)
    {
        Vector3 perpendicularDir = Vector3.Cross(swordDirection, Vector3.up).normalized;

        Rigidbody upperRb = upperHull.GetComponent<Rigidbody>();
        Rigidbody lowerRb = lowerHull.GetComponent<Rigidbody>();

        upperRb.AddForce(perpendicularDir * SliceForce);
        lowerRb.AddForce(-perpendicularDir * SliceForce);
    }

    private IEnumerator ScheduleDestruction(GameObject hull)
    {
        if (hull != null)
        {
            yield return new WaitForSeconds(3f);
            Destroy(hull, DestructionDelay);
        }
    }
}
