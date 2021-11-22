using UnityEngine;
using UnityEngine.Events;

public class Invoker : MonoBehaviour
{
	public UnityEvent Events;
	public KeyCode Activator;


	private void Update()
	{
		if(Input.GetKeyDown(Activator))
		{
			Events.Invoke();
		}
	}
}
