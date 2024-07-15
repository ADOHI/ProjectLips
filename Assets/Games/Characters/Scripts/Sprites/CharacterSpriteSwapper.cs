using DG.Tweening;
using PL.Systems.Bosses;
using PL.Systems.Grids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PL.Systems.Characters.Sprites
{
    public class CharacterSpriteSwapper : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public Sprite upSprite;
        public Sprite downSprite;
        public Sprite leftSprite;
        public Sprite rightSprite;
        public Sprite attackSprite;

        public float rotateDuration = 0.2f;
        void Start()
        {
            CharacterManager.Instance.character.moveController.onCharacterMoveStart.AddListener((first, second) => SwapSprite(second));
            CharacterManager.Instance.character.moveController.onCharacterMoveStart.AddListener(RotateSprite);

            BossManager.Instance.leftHand.onGrapCharacterStart.AddListener(_ => Attacked());
            BossManager.Instance.rightHand.onGrapCharacterStart.AddListener(_ => Attacked());

            CharacterManager.Instance.character.moveController.onReleaseGrap.AddListener(SwapSprite);
        }

        public void SwapSprite(GridMap.GridDirection direction)
        {
            switch (direction)
            {
                case GridMap.GridDirection.Up:
                    spriteRenderer.sprite = upSprite;
                    break;
                case GridMap.GridDirection.Down:
                    spriteRenderer.sprite = downSprite;
                    break;
                case GridMap.GridDirection.Left:
                    spriteRenderer.sprite = leftSprite;
                    break;
                case GridMap.GridDirection.Right:
                    spriteRenderer.sprite = rightSprite;
                    break;
                case GridMap.GridDirection.None:
                    break;
            }
        }

        public void Attacked()
        {
            spriteRenderer.sprite = attackSprite;
        }

        public void RotateSprite(GridMap.GridDirection prevDirection, GridMap.GridDirection direction)
        {
            if (prevDirection == direction)
            {
                return;
            }

            switch (prevDirection)
            {
                case GridMap.GridDirection.Up:
                    switch (direction)
                    {
                        case GridMap.GridDirection.Down:
                            spriteRenderer.transform.DORotate(new Vector3(0f, 0f, 0f), rotateDuration).From(new Vector3(0f, 180f, 0f));
                            break;
                        case GridMap.GridDirection.Left:
                            spriteRenderer.transform.DORotate(new Vector3(0f, 0f, 0f), rotateDuration).From(new Vector3(0f, 90f, 0f));
                            break;
                        case GridMap.GridDirection.Right:
                            spriteRenderer.transform.DORotate(new Vector3(0f, 0f, 0f), rotateDuration).From(new Vector3(0f, -90f, 0f));
                            break;
                    }
                    break;
                case GridMap.GridDirection.Down:
                    switch (direction)
                    {
                        case GridMap.GridDirection.Up:
                            spriteRenderer.transform.DORotate(new Vector3(0f, 0f, 0f), rotateDuration).From(new Vector3(0f, 180f, 0f));
                            break;
                        case GridMap.GridDirection.Left:
                            spriteRenderer.transform.DORotate(new Vector3(0f, 0f, 0f), rotateDuration).From(new Vector3(0f, -90f, 0f));
                            break;
                        case GridMap.GridDirection.Right:
                            spriteRenderer.transform.DORotate(new Vector3(0f, 0f, 0f), rotateDuration).From(new Vector3(0f, 90f, 0f));
                            break;
                    }
                    break;
                case GridMap.GridDirection.Left:
                    switch (direction)
                    {
                        case GridMap.GridDirection.Up:
                            spriteRenderer.transform.DORotate(new Vector3(0f, 0f, 0f), rotateDuration).From(new Vector3(0f, -90f, 0f));
                            break;
                        case GridMap.GridDirection.Down:
                            spriteRenderer.transform.DORotate(new Vector3(0f, 0f, 0f), rotateDuration).From(new Vector3(0f, 90f, 0f));
                            break;
                        case GridMap.GridDirection.Right:
                            spriteRenderer.transform.DORotate(new Vector3(0f, 0f, 0f), rotateDuration).From(new Vector3(0f, 180f, 0f));
                            break;
                    }
                    break;
                case GridMap.GridDirection.Right:
                    switch (direction)
                    {
                        case GridMap.GridDirection.Up:
                            spriteRenderer.transform.DORotate(new Vector3(0f, 0f, 0f), rotateDuration).From(new Vector3(0f, 90f, 0f));
                            break;
                        case GridMap.GridDirection.Down:
                            spriteRenderer.transform.DORotate(new Vector3(0f, 0f, 0f), rotateDuration).From(new Vector3(0f, -90f, 0f));
                            break;
                        case GridMap.GridDirection.Left:
                            spriteRenderer.transform.DORotate(new Vector3(0f, 0f, 0f), rotateDuration).From(new Vector3(0f, 180f, 0f));
                            break;
                    }
                    break;
                case GridMap.GridDirection.None:
                    break;
            }
        }
    }

}
