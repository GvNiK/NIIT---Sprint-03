using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Pool<T>
{
	private Func<T> CreateElement;
	private List<T> pool;
	private List<T> inUse;
	private Action<T> OnRemovedFromPool;
	private Action<T> OnReturnedToPool;

	public Pool(Func<T> CreateElement,
		Action<T> OnRemovedFromPool, Action<T> OnReturnedToPool)
	{
		this.CreateElement = CreateElement;
		this.OnRemovedFromPool = OnRemovedFromPool;
		this.OnReturnedToPool = OnReturnedToPool;
		pool = new List<T>();
		inUse = new List<T>();
	}

	public T Get()
	{
		if(pool.Count == 0)
		{
			Add(CreateElement());
		}

		T element = pool[0];
		pool.RemoveAt(0);
		inUse.Add(element);
		OnRemovedFromPool(element);

		return element;
	}

	public void Return(T element)
	{
		inUse.Remove(element);
		Add(element);
	}

	private void Add(T element)
	{
		pool.Add(element);
		OnReturnedToPool(element);
	}
}
