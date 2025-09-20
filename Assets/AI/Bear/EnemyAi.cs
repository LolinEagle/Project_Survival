using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour{
	[Header("Reference")]
	[SerializeField] private NavMeshAgent	agent;
	[SerializeField] private Animator		animator;
	[SerializeField] private AudioSource	mainAudioSource;
	[SerializeField] private AudioSource	audioSource;
	[SerializeField] private Transform		dropPoint;
	[SerializeField] private Ressource[]	lootTable;

	[Header("Stats")]
	[SerializeField] private float	maxHealth;
	[SerializeField] private float	currentHealth;
	[SerializeField] private float	walkSpeed;
	[SerializeField] private float	chaseSpeed;
	[SerializeField] private float	detectionRadius;
	[SerializeField] private float	attackRadius;
	[SerializeField] private float	attackDelay;
	[SerializeField] private float	damageDealt;
	[SerializeField] private float	rotationSpeed;

	[Header("Wandering parameters")]
	[SerializeField] private float	wanderingWaitTimeMin;
	[SerializeField] private float	wanderingWaitTimeMax;
	[SerializeField] private float	wanderingDistanceMin;
	[SerializeField] private float	wanderingDistanceMax;

	[Header("Nav Mesh parameters")]
	[SerializeField] private float	navMeshCheckRadius = 2f;
	[SerializeField] private float	navMeshRetryInterval = 0.25f;

	private Transform	player;
	private PlayerStats	playerStats;
	private bool		hasDestination;
	private bool		isAttacking;
	private bool		isDead;

	private void		Awake(){
		Transform	playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

		player = playerTransform;
		playerStats = playerTransform.GetComponent<PlayerStats>();
		currentHealth = maxHealth;

		// Ensure agent reference and keep it disabled by default if desired
		if (!agent) agent = GetComponent<NavMeshAgent>();
		// agent.enabled should be false in the Inspector; if not:
		// agent.enabled = false;
	}

	private void		Start(){
		// Try to enable the agent once it's on a valid NavMesh
		StartCoroutine(EnsureAgentOnNavMesh());
	}

	private IEnumerator	EnsureAgentOnNavMesh(){
		// Keep trying in case the NavMesh is built at runtime (e.g., NavMeshSurface.BuildNavMeshAsync)
		while (true){
			if (!agent.enabled){
				if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, navMeshCheckRadius, NavMesh.AllAreas)){
					agent.enabled = true;
					agent.Warp(hit.position); // snap onto the NavMesh to avoid off-mesh placement
					break;
				}
			}else{
				// Already enabled (e.g., placed correctly in editor)
				break;
			}
			yield return new WaitForSeconds(navMeshRetryInterval);
		}
	}

	void				Update(){
		// Guard: do nothing until the agent is enabled and usable
		if (!agent || !agent.enabled) {
			animator.SetFloat("Speed", 0f);
			return;
		}

		if (Vector3.Distance(player.position, transform.position) < detectionRadius && !playerStats.isDead){
			agent.speed = chaseSpeed;
			Quaternion	rot = Quaternion.LookRotation(player.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
			if (!isAttacking){
				if (Vector3.Distance(player.position, transform.position) < attackRadius) {
					StartCoroutine(AttackPlayer());
				} else {
					agent.SetDestination(player.position);
				}
			}
		} else {
			agent.speed = walkSpeed;
			if (agent.remainingDistance < 0.75f && !hasDestination){
				StartCoroutine(GetNewDestinasion());
			}
		}
		animator.SetFloat("Speed", agent.velocity.magnitude);
	}

	void				OnDeath(){
		isDead = true;
		animator.SetTrigger("Die");

		// Disable the agent and this script
		if (agent) agent.enabled = false;
		mainAudioSource.enabled = false;
		enabled = false;

		// Drop loot
		for (int i = 0; i < lootTable.Length; i++){
			Ressource	ressource = lootTable[i];
			if (Random.Range(1, 101) <= ressource.dropChance){
				GameObject	instantiated = Instantiate(ressource.itemData.prefab);
				instantiated.transform.position = dropPoint.position;
			}
		}
	}

	public bool			TakeDamage(float damages){
		if (isDead) return false;

		// Check for death
		currentHealth -= damages;
		if (currentHealth <= 0){
			OnDeath();
			return true;
		}

		// Is not dead, play hit reaction
		animator.SetTrigger("GetHit");
		return false;
	}

	IEnumerator			GetNewDestinasion(){
		hasDestination = true;
		yield return new WaitForSeconds(Random.Range(wanderingWaitTimeMin, wanderingWaitTimeMax));

		Vector3 nextDestination = transform.position;
		nextDestination += Random.Range(wanderingDistanceMin, wanderingDistanceMax) * new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

		if (NavMesh.SamplePosition(nextDestination, out NavMeshHit hit, wanderingDistanceMax, NavMesh.AllAreas)){
			agent.SetDestination(hit.position);
		}
		hasDestination = false;
	}

	IEnumerator			AttackPlayer(){
		isAttacking = true;
		if (agent && agent.enabled) agent.isStopped = true;
		audioSource.Play();
		animator.SetTrigger("Attack");
		playerStats.TakeDamage(damageDealt);

		yield return new WaitForSeconds(attackDelay);
		if (agent && agent.enabled){
			agent.isStopped = false;
		}
		isAttacking = false;
	}

	private void		OnDrawGizmos(){
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, detectionRadius);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRadius);
	}
}
