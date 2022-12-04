using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionShipHandler : MonoBehaviour, IMinionShip
{
    IMothership _mothership;
    MindsetHandler _mindsetHandler;
    ParticleController _particleController;

    public void InitializeWithAssignedMothership(IMothership mothership, Transform mothershipTransform)
    {
        _particleController = FindObjectOfType<ParticleController>();
        _mothership = mothership;
        GetComponentInChildren<DetectionHandler>().PlayerTransformUpdated += IssueAlertToMothership;
        _mindsetHandler = GetComponentInParent<MindsetHandler>();
        GetComponent<Mindset_Explore>().SetDependentTransform(mothershipTransform);
        GetComponent<HealthHandler>().Dying += HandleMinionAspectsOfDying;
    }

    private void IssueAlertToMothership(Vector3 targetPosition, Vector3 targetVelocity)
    {
        _mothership.AlertAllMinionsToTargetTransform(targetPosition, targetVelocity);
    }

    public void AssignTarget(Vector3 targetPosition, Vector3 targetVelocity)
    {
        _mindsetHandler.HandlePlayerTransformUpdated(targetPosition, targetVelocity);
    }

    private void HandleMinionAspectsOfDying()
    {
        _particleController?.RequestBlastParticles(5, 3f, transform.position);
        _mothership?.RemoveDeadMinion(this);
        Destroy(gameObject);
    }

    public void KillMinionUponMothershipDeath()
    {
        HandleMinionAspectsOfDying();
    }
}
