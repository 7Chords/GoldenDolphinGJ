public interface IDamagable
{
    public void Attack();
    public void TakeDamage(int damage);
    public int GetAttackAmount();

    public void Dead();
}
