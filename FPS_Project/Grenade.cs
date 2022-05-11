using Linefy;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
	[SerializeField]
	private MMObjectPooler hitObjectPooler;
	[SerializeField]
	private Color colorLine;

	public enum State
	{
		waitSpawn,
		animation,
		animationFade
	}

	public State state = State.waitSpawn;

	public float DisableFadeLength = 3;
	public float DisableAltitude = -3.8f;

	private float disableFadeTimer;
	private Polyline pathPolyline;
	private int updateEveryFrame = 3;
	private int updateCounter;
	private float overallTransparency;
	private float visibleDistance = 10;
	private Rigidbody rb;
	private float trajectoryWidth;
	private GameObject owner;

	public bool IsActive { get; private set; }

    public void Init(Transform spawnPoint, Texture2D trajectoryTexture, float trajectoryWidth, Vector3 forceToAdd, GameObject owner)
	{
		rb = GetComponent<Rigidbody>();
		this.trajectoryWidth = trajectoryWidth;
		this.owner = owner;

		pathPolyline = new Polyline(name, 0, true, 1, false, trajectoryTexture, 4, 128);
		pathPolyline.widthMode = WidthMode.PercentOfScreenHeight;

		gameObject.SetActive(true);
		disableFadeTimer = 0;
		rb.AddForce(forceToAdd);

		transform.position = spawnPoint.position;

		pathPolyline.AddWithDistance(new PolylineVertex(transform.position, colorLine, trajectoryWidth));
		updateCounter = 0;
		overallTransparency = 1;
		IsActive = true;
		state = State.animation;
	}

	public void Update()
	{
		if (state == State.animation)
		{
			if (transform.position.y < DisableAltitude)
			{
				state = State.animationFade;
				disableFadeTimer = DisableFadeLength;
			}
		}
		else if (state == State.animationFade)
		{
			overallTransparency = disableFadeTimer / DisableFadeLength;
			disableFadeTimer -= Time.deltaTime;
			if (disableFadeTimer < 0)
			{
				DisableLine();
			}
		}

		if (state != State.waitSpawn)
		{
			if (updateCounter == updateEveryFrame)
			{
				pathPolyline.AddWithDistance(new PolylineVertex(transform.position, colorLine, trajectoryWidth));
				updateCounter = 0;
				float totalPathLength = pathPolyline[pathPolyline.count - 1].textureOffset;
				float vd = Mathf.Min(visibleDistance, totalPathLength);
				for (int i = 0; i < pathPolyline.count; i++)
				{
					float d = pathPolyline.GetDistance(i);
					float op = (1f - (totalPathLength - d) / vd) * overallTransparency;
					pathPolyline.SetAlpha(i, op);
				}
			}
			else
			{
				updateCounter++;
			}
			pathPolyline.Draw();
		}
	}

	private void OnCollisionEnter(Collision collision)
    {
		if (IsActive)
        {
			OnHit();
			DisableLine();
			Disable();
		}
	}

	protected virtual void OnHit()
	{
		GameObject pooledObj = hitObjectPooler.GetPooledGameObject();
		pooledObj.transform.position = this.transform.position;
		
		AreaEffect areaEffect = pooledObj.GetComponent<AreaEffect>();
		if (areaEffect)
			areaEffect.SetOwner(owner);
		
		pooledObj.SetActive(true);
	}

	public void DisableLine()
	{
		pathPolyline.count = 0;
		state = State.waitSpawn;
	}

	public void Disable()
	{
		gameObject.SetActive(false);
		IsActive = false;
	}
}
