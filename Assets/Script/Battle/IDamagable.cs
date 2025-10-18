public interface IDamagable
{
    public void Attack();
    public void TakeDamage(int damage);
    public int GetAttackAmount();
    public void TakeHeal(int healAmount);
    public int GetHealAmount();
    public void TakeBuff(int buffAmount);
    public int GetBuffAmount();
    public void Dead();
}
