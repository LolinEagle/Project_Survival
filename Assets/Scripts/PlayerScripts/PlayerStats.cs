using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour{
	[Header("Other elements references")]
	[SerializeField] private Animator			animator;
	[SerializeField] private MoveBehaviour		playerMovementScript;

	public float currentHealth;
	public float currentHunger;
	public float currentThirs;

	[SerializeField] private float maxHealth = 100f;
	[SerializeField] private float maxHunger = 100f;
	[SerializeField] private float maxThirs = 100f;

	[SerializeField] private Image BarFillHealth;
	[SerializeField] private Image BarFillHunger;
	[SerializeField] private Image BarFillThirs;

	[SerializeField] private float healthDecreaseRate;
	[SerializeField] private float hungerDecreaseRate;
	[SerializeField] private float thirsDecreaseRate;

	public float	currentArmorPoints;
	public bool		isDead = false;

	void Awake(){
		currentHealth = maxHealth;
		currentHunger = maxHunger;
		currentThirs = maxThirs;
	}

	void Update(){
		UpdateBarFill();
	}

	private void	Die(){
		isDead = true;
		playerMovementScript.canMove = false;
		hungerDecreaseRate = 0;
		thirsDecreaseRate = 0;
		animator.SetTrigger("Die");
	}

	public void		TakeDamage(float damage, bool overtime = false){
		if (overtime) {
			currentHealth -= damage * Time.deltaTime;
		} else {
			currentHealth -= damage * (1 - (currentArmorPoints / 100));
		}
		UpdateHealthBarFill();

		if (currentHealth <= 0 && !isDead){
			Die();
		}
	}

	public void		UpdateHealthBarFill(){
		BarFillHealth.fillAmount = currentHealth / maxHealth;
	}

	void			UpdateBarFill(){
		currentHunger -= hungerDecreaseRate * Time.deltaTime;
		currentThirs -= thirsDecreaseRate * Time.deltaTime;
		currentHunger = currentHunger < 0 ? 0 : currentHunger;
		currentThirs = currentThirs < 0 ? 0 : currentThirs;
		BarFillHunger.fillAmount = currentHunger / maxHunger;
		BarFillThirs.fillAmount = currentThirs / maxThirs;

		if (currentHunger <= 0 || currentThirs <= 0){
			TakeDamage((currentHunger <= 0 && currentThirs <= 0 ? healthDecreaseRate * 2 : healthDecreaseRate), true);
		}
	}

	public void		Consumeitem(float health, float hunger, float thirs){
		currentHealth += health;
		if (currentHealth > maxHealth){
			currentHealth = maxHealth;
		} else if (currentHealth < 0){
			currentHealth = 0;
		}

		currentHunger += hunger;
		if (currentHunger > maxHunger){
			currentHunger = maxHunger;
		}

		currentThirs += thirs;
		if (currentThirs > maxThirs){
			currentThirs = maxThirs;
		}

		UpdateHealthBarFill();
	}
}
