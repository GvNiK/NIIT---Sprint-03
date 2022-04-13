using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Tween
{
	public Action OnComplete = delegate { };
	public abstract void Start();
	public abstract void Update();
}
