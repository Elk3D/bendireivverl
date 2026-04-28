using UnityEngine;

public class SeasonalCauldron : ActionEventController<SeasonalCauldronContent, SeasonalCauldronData>
{
	[SerializeField]
	private ParticleSystem[] m_Particles;

	[SerializeField]
	private GameObject m_Deposit;

	public void EnableParticles()
	{
		for (int i = 0; i < m_Particles.Length; i++)
		{
			m_Particles[i].Play();
		}
	}

	public void DisableParticles()
	{
		for (int i = 0; i < m_Particles.Length; i++)
		{
			m_Particles[i].Stop();
		}
	}

	public void Deposit()
	{
		m_Deposit.gameObject.SetActive(value: true);
		Invoke("DepositComplete", 10f);
	}

	private void DepositComplete()
	{
		m_Deposit.gameObject.SetActive(value: false);
	}
}
