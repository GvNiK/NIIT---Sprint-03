using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VFXPool
{
	private Dictionary<string, Pool<ParticleSystem>> pools;
	private GameObject container;
	private VFXLibrary library;

	public VFXPool(VFXLibrary library)
	{
		this.library = library;
		container = new GameObject("VFX Container");
		pools = new Dictionary<string, Pool<ParticleSystem>>();
	}

	public ParticleSystem Get(string ID)
	{
		return GetPool(ID).Get();
	}

	private Pool<ParticleSystem> GetPool(string ID)
	{
		if (pools.ContainsKey(ID) == false)
		{
			pools[ID] = new Pool<ParticleSystem>(() =>
			{
				GameObject instance = GameObject.Instantiate(library.Find(ID).obj, container.transform);
				return instance.GetComponent<ParticleSystem>();
			},
			(vfx) => vfx.gameObject.SetActive(true),
			(vfx) => vfx.gameObject.SetActive(false));
		}

		return pools[ID];
	}

	public void Return(string ID, ParticleSystem vfx)
	{
		GetPool(ID).Return(vfx);
	}
}
