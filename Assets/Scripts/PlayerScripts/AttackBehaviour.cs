using UnityEngine;

public class AttackBehaviour : MonoBehaviour{
	[Header("References")]
	[SerializeField] private Animator			animator;
	[SerializeField] private Equipment			equipment;
	[SerializeField] private UIManager			manager;
	[SerializeField] private InteractBehaviour	interactBehaviour;
	[SerializeField] private PlayerStats		playerStats;

	[Header("Configuration")]
	[SerializeField] private float		attackRange;
	[SerializeField] private LayerMask	layerMask;
	[SerializeField] private Vector3	attackOffset;

	private bool	isAttacking;

	void		Update(){
		// Debug.DrawRay(transform.position + attackOffset, transform.forward * attackRange, Color.red);
		if (Input.GetMouseButtonDown(0) && CanAttack()){
			isAttacking = true;
			SendAttack();
			animator.SetTrigger("Attack");
		}
	}

	void		SendAttack(){
		RaycastHit	hit;

		if (Physics.Raycast(transform.position + attackOffset, transform.forward, out hit, attackRange, layerMask)){
			if (hit.transform.CompareTag("Ai")){
				EnemyAi enemy = hit.transform.GetComponent<EnemyAi>();
				enemy.TakeDammge(equipment.equipedWeaponItem.attackPoints);
			}
		}
	}

	bool		CanAttack(){
		return (equipment.equipedWeaponItem != null && !isAttacking && !manager.isAPanelOpened && !interactBehaviour.isBusy && !playerStats.isDead);
	}

	public void	AttackEnd(){
		isAttacking = false;
	}
}
