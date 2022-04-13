using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ProjectilePool
{
	public Action<IProjectile> OnRemovedFromPool = delegate { };
	public Action<IProjectile> OnReturnedToPool = delegate { };
	private Dictionary<ProjectileType, Pool<GameObject>> pools;
	private GameObject container;
	private ProjectileFactory factory;

	public ProjectilePool(ProjectileLibrary library, ProjectileFactory factory)
	{
		this.factory = factory;
		container = new GameObject("Projectile Container");
		pools = new Dictionary<ProjectileType, Pool<GameObject>>();
		foreach (ProjectileType type in Enum.GetValues(typeof(ProjectileType)))
		{
			pools[type] = new Pool<GameObject>(() =>
			{
				return GameObject.Instantiate(library.Find(type).obj); 
			},
			(projectile) => projectile.SetActive(true),
			(projectile) =>
			{
				projectile.SetActive(false);
				projectile.transform.SetParent(container.transform);
				Rigidbody body = projectile.GetComponent<Rigidbody>();
				body.velocity = body.angularVelocity = Vector3.zero;
				body.rotation = Quaternion.Euler(Vector3.zero);
			});
		}
	}

	public IProjectile Get(ProjectileType type)
	{
		GameObject obj = pools[type].Get();
		IProjectile newProjectile = factory.Create(type, obj, this);
		newProjectile.OnDestroyRequested += () => Return(newProjectile);

		OnRemovedFromPool(newProjectile);
		return newProjectile;
	}

	public void Return(IProjectile projectile)
	{
		pools[projectile.Type].Return(projectile.Transform.gameObject);
		OnReturnedToPool(projectile);
	}
}
