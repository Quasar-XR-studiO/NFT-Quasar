using System.Collections;
using System.Collections.Generic;
using AllArt.Solana.Nft;
using MalbersAnimations;
using Rarible;
using UnityEngine;

public abstract class BaseSolanaNftItemController : MonoBehaviour
{
    public Nft Nft  = new Nft();

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
    }

    public virtual void Create(Nft nft)
    {
        Nft = nft;
    }

    public virtual void Create()
    {
        Create(Nft);
    }
}