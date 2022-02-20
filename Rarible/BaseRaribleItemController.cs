using System.Collections;
using System.Collections.Generic;
using MalbersAnimations;
using Rarible;
using UnityEngine;

public abstract class BaseRaribleItemController : MonoBehaviour
{
    [HideInInspector] public RaribleItem RaribleItem = new RaribleItem();
    [HideInInspector] public Content OriginalContent = new Content();

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
    }

    public void CreateWithDelay(RaribleItem raribleItem, Content content)
    {
        if (gameObject.activeSelf)
        {
            Create(raribleItem, content);
        }
        else
        {
            StartCoroutine(CreateWithDelayCoroutine(raribleItem, content));
        }
    }

    private IEnumerator CreateWithDelayCoroutine(RaribleItem raribleItem, Content content)
    {
        yield return new WaitForSeconds(1f);
        CreateWithDelay(raribleItem, content);
    }

    public virtual void Create(RaribleItem raribleItem, Content originalContent)
    {
        RaribleItem = raribleItem;
        OriginalContent = originalContent;
    }

    public virtual void Create()
    {
        Create(RaribleItem, OriginalContent);
    }
}