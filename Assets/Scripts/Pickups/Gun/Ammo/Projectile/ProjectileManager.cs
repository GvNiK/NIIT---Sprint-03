using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ProjectileManager
{
	private List<IProjectile> activeProjectiles;
	private List<IProjectile> toRemoveCache;

	public ProjectileManager(ProjectilePool pool)
	{
		activeProjectiles = new List<IProjectile>();
		toRemoveCache = new List<IProjectile>();
		pool.OnRemovedFromPool += (projectile) =>
		{
			activeProjectiles.Add(projectile);
		};
		pool.OnReturnedToPool += (projectile) =>
		{
			toRemoveCache.Add(projectile);
		};
	}

	public void Update()
	{
		foreach (IProjectile projectile in toRemoveCache)
		{
			activeProjectiles.Remove(projectile);
		}
		toRemoveCache.Clear();

		foreach(IProjectile projectile in activeProjectiles)
		{
			projectile.Update();
		}
	}
}
