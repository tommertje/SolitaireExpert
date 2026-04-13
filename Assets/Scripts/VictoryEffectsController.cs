using System.Collections;
using UnityEngine;

public class VictoryEffectsController : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private int fireworkBursts = 9;
    [SerializeField] private float burstIntervalSeconds = 0.16f;
    [SerializeField] private float fireworkDepth = 10f;
    [SerializeField] private float celebrationVolume = 0.7f;

    private AudioClip happyClip;
    private Coroutine celebrationRoutine;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }

    public void PlayVictoryCelebration()
    {
        if (celebrationRoutine != null)
        {
            StopCoroutine(celebrationRoutine);
        }

        celebrationRoutine = StartCoroutine(PlayVictoryCelebrationRoutine());
    }

    private IEnumerator PlayVictoryCelebrationRoutine()
    {
        PlayHappyJingle();

        for (int i = 0; i < fireworkBursts; i++)
        {
            SpawnFireworkBurst(GetRandomFireworkPosition());
            yield return new WaitForSeconds(burstIntervalSeconds);
        }

        celebrationRoutine = null;
    }

    private void PlayHappyJingle()
    {
        if (happyClip == null)
        {
            happyClip = CreateHappyClip();
        }

        audioSource.PlayOneShot(happyClip, celebrationVolume);
    }

    private AudioClip CreateHappyClip()
    {
        const int sampleRate = 44100;
        const float noteDuration = 0.12f;
        float[] notes = new float[]
        {
            523.25f, 659.25f, 783.99f, 1046.50f, 783.99f, 1046.50f
        };

        int samplesPerNote = Mathf.RoundToInt(sampleRate * noteDuration);
        float[] data = new float[samplesPerNote * notes.Length];

        int offset = 0;
        for (int i = 0; i < notes.Length; i++)
        {
            float frequency = notes[i];
            for (int s = 0; s < samplesPerNote; s++)
            {
                float time = s / (float)sampleRate;
                float sample = Mathf.Sin(2f * Mathf.PI * frequency * time);
                float envelope = Mathf.Clamp01(s / (sampleRate * 0.01f)) *
                                 Mathf.Clamp01((samplesPerNote - s) / (sampleRate * 0.02f));
                data[offset + s] = sample * 0.25f * envelope;
            }

            offset += samplesPerNote;
        }

        AudioClip clip = AudioClip.Create("VictoryHappyJingle", data.Length, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private Vector3 GetRandomFireworkPosition()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
        {
            return Vector3.zero;
        }

        float viewportX = Random.Range(0.12f, 0.88f);
        float viewportY = Random.Range(0.58f, 0.92f);
        return targetCamera.ViewportToWorldPoint(new Vector3(viewportX, viewportY, fireworkDepth));
    }

    private void SpawnFireworkBurst(Vector3 position)
    {
        GameObject firework = new GameObject("VictoryFireworkBurst");
        firework.transform.position = position;

        ParticleSystem particles = firework.AddComponent<ParticleSystem>();
        ConfigureParticleSystem(particles);
        particles.Play();

        Destroy(firework, 2.2f);
    }

    private void ConfigureParticleSystem(ParticleSystem particles)
    {
        var main = particles.main;
        main.duration = 0.9f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.6f, 1.1f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(2.5f, 5.2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.11f);
        main.maxParticles = 90;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 0.28f;
        main.startColor = Random.ColorHSV(0f, 1f, 0.75f, 1f, 0.8f, 1f);

        var emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, (short)48, (short)72)
        });

        var shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.08f;

        var colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient colorGradient = new Gradient();
        colorGradient.SetKeys(
            new[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Random.ColorHSV(0f, 1f, 0.75f, 1f, 0.8f, 1f), 0.2f),
                new GradientColorKey(Color.black, 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.85f, 0.75f),
                new GradientAlphaKey(0f, 1f)
            });
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(colorGradient);
    }
}
