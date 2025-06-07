using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    [SerializeField] private BaseMachine _machine;
    public int ClonesPerSecond = 10;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform tf;
    [SerializeField] private List<GameObject> clones;
    public Vector3 scalePerSecond = new Vector3(1f, 1f, 1f);
    public Color colorPerSecond = new Color(255, 255, 255, 1f);
    void Start()
    {
        _machine = GetComponent<BaseMachine>();
        rb = GetComponent<Rigidbody2D>();
        tf = GetComponent<Transform>();
        sr = _machine.Caterpillar.sprite; //GetComponent<SpriteRenderer>();
        clones = new List<GameObject>();
        StartCoroutine(trail());
    }

    void Update()
    {
        for (int i = 0; i < clones.Count; i++)
        {
            clones[i].transform.localScale -= scalePerSecond  * Time.deltaTime;
            if (clones[i].transform.localScale.x < Vector3.zero.x) //clones[i].color.a <= 0f || 
            {
                Destroy(clones[i].gameObject);
                clones.RemoveAt(i);
                i--;
                continue;
            }
        
            SpriteRenderer[] sps = clones[i].GetComponentsInChildren<SpriteRenderer>();

            for (int j = 0; j < sps.Length; j++)
            {
                sps[j].color -= colorPerSecond * Time.deltaTime;
                if (sps[j].color.a <= 0f)
                {
                    Destroy(clones[i].gameObject);
                    clones.RemoveAt(i);
                    i--;
                    break;
                }
            }
        }
    }

    IEnumerator trail()
    {
        for (; ; ) //while(true)
        {
            if (rb.linearVelocity != Vector2.zero)
            {
                var clone = Instantiate(_machine.Caterpillar.trailPrefab); // new GameObject("trailClone");
                clone.transform.position = tf.position;
                clone.transform.localScale = tf.localScale;
                clone.transform.rotation = _machine.Body.transform.rotation;
                // var cloneRend = clone.AddComponent<SpriteRenderer>();
                // cloneRend.sprite = sr.sprite;
                // cloneRend.sortingOrder = sr.sortingOrder - 1;
                clones.Add(clone);
            }
            yield return new WaitForSeconds(1f / ClonesPerSecond);
        }
    }
}