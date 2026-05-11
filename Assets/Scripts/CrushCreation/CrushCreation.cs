// using System;
// using System.Collections.Generic;
// using Unity.Netcode;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class CrushCreation : NetworkBehaviour
// {
//     public Image hair, face, body, accessories;
//     public List<Sprite> hairSprites,  faceSprites, bodySprites, accessoriesSprites;
//     public GameObject canvasCrush;
//     public GameManager gameManager;
//     public PlayerNetwork playerRef;
//     
//     private int _hairIndex, _faceIndex, _bodyIndex, _accessoriesIndex;
//
//     private void Start()
//     {
//         gameManager = FindFirstObjectByType<GameManager>();
//         hair.sprite = hairSprites[_hairIndex];
//         face.sprite = faceSprites[_faceIndex];
//         body.sprite = bodySprites[_bodyIndex];
//         accessories.sprite = accessoriesSprites[_accessoriesIndex];
//     }
//
//     public void ChangeHair(bool increase)
//     {
//         if (increase)
//         {
//             _hairIndex++;
//             if (_hairIndex >= hairSprites.Count)
//             {
//                 _hairIndex = 0;
//             }
//         }
//         else
//         {
//             _hairIndex--;
//             if (_hairIndex < 0)
//             {
//                 _hairIndex = hairSprites.Count - 1;
//             }
//         }
//         
//         hair.sprite = hairSprites[_hairIndex];
//     }
//     
//     public void ChangeFace(bool increase)
//     {
//         if (increase)
//         {
//             _faceIndex++;
//             if (_faceIndex >= faceSprites.Count)
//             {
//                 _faceIndex = 0;
//             }
//         }
//         else
//         {
//             _faceIndex--;
//             if (_faceIndex < 0)
//             {
//                 _faceIndex = faceSprites.Count - 1;
//             }
//         }
//         
//         face.sprite = faceSprites[_faceIndex];
//     }
//     
//     public void ChangeBody(bool increase)
//     {
//         if (increase)
//         {
//             _bodyIndex++;
//             if (_bodyIndex >= bodySprites.Count)
//             {
//                 _bodyIndex = 0;
//             }
//         }
//         else
//         {
//             _bodyIndex--;
//             if (_bodyIndex < 0)
//             {
//                 _bodyIndex = bodySprites.Count - 1;
//             }
//         }
//         
//         body.sprite = bodySprites[_bodyIndex];
//     }
//     public void ChangeAccessories(bool increase)
//     {
//         if (increase)
//         {
//             _accessoriesIndex++;
//             if (_accessoriesIndex >= accessoriesSprites.Count)
//             {
//                 _accessoriesIndex = 0;
//             }
//         }
//         else
//         {
//             _accessoriesIndex--;
//             if (_accessoriesIndex < 0)
//             {
//                 _accessoriesIndex = accessoriesSprites.Count - 1;
//             }
//         }
//         
//         accessories.sprite = accessoriesSprites[_accessoriesIndex];
//     }
//     
//     public void CloseCrushCreation()
//     {
//         if (IsServer)
//         {
//             canvasCrush.SetActive(false);
//         }
//         OpenMiniGameUIClientRpc();
//     }
//
//     [ClientRpc]
//     public void OpenMiniGameUIClientRpc()
//     {
//         if (!IsServer)
//         {
//             playerRef.canvasBody.SetActive(false);
//             playerRef.canvasHair.SetActive(false);
//             playerRef.canvasFace.SetActive(false);
//             playerRef.canvasAccessories.SetActive(false);
//             playerRef.canvasJump.SetActive(true);
//         }
//     }
// }
