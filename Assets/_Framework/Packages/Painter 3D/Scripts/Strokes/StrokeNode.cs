﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EXP.Painter
{
    // A brush stroke is composed of stroke nodes
    // Eahc node stores it's original transform along and extra transform attributes so that it can be updated and move
    [System.Serializable]
    public class StrokeNode
    {
        Space _Space = Space.Self;

        #region Original transform values

        public Vector3 OriginalPos { get; set; }
        public Quaternion OriginalRot { get; set; }
        public Vector3 OriginalScale { get; set; }

        public Vector3 JitterPos { get; set; }
        public Quaternion JitterRot { get; set; }       
        public Vector3 JitterScale { get; set; }

        public Vector3 OffsetPos { get; set; }
        public Quaternion OffsetRot { get; set; }
        public Vector3 OffsetScale { get; set; }

        #endregion


        #region Modified transform values
       
        public Vector3 ModifiedPos
        {
            get
            {
                if (_Space == Space.World)
                {
                    return OriginalPos + OffsetPos + JitterPos;
                }
                else
                {
                    return OriginalPos + (ModifiedRot * (OffsetPos + JitterPos));
                }
            }
        }
        public Quaternion ModifiedRot { get { return OriginalRot * OffsetRot * JitterRot; } }
        public Vector3 ModifiedScale { get { return OriginalScale + OffsetScale + JitterScale; } }

        #endregion

        #region Constructor methods
        public StrokeNode(Transform transform)
        {
            OriginalPos = transform.position;
            OriginalRot = transform.rotation;
            OriginalScale = transform.localScale;
        }

        public Vector3 tanget;
        public Vector3 normal;
        public Vector3 binormal;
        public StrokeNode(Transform brushTipT, Transform strokeTransform, StrokeNode prevNode = null)
        {
            Transform preParent = brushTipT.parent;
            brushTipT.SetParent(strokeTransform);

            OriginalPos = brushTipT.localPosition;          
            OriginalScale = brushTipT.localScale;

            if (prevNode == null)
            {
                tanget = brushTipT.forward;
                normal = Vector3.Cross(tanget, Vector3.up).normalized;
                binormal = Vector3.Cross(tanget, normal).normalized;
            }
            else
            {
                tanget = (brushTipT.position - prevNode.OriginalPos).normalized;
                normal = Vector3.Cross(prevNode.binormal, tanget).normalized;
                binormal = Vector3.Cross(tanget, normal).normalized;
            }

            if (prevNode != null)
                OriginalRot = TransformExtensions.SmoothRotation(brushTipT.position, prevNode.OriginalPos, prevNode.OriginalRot);
            else
                OriginalRot = Quaternion.LookRotation(tanget, normal);


            brushTipT.SetParent(preParent);
        }

        public StrokeNode(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            OriginalPos = pos;
            OriginalRot = rot;
            OriginalScale = scale;
        }
        #endregion


        #region Helper methods
        
        public void SetOriginalRotation(Quaternion r)
        {
            OriginalRot = r;
        }

        public void SetTransform(Transform t, bool local)
        {
            if (local)
            {
                t.localPosition = ModifiedPos;
                t.localRotation = ModifiedRot;
                t.localScale = ModifiedScale;
            }
            else
            {
                t.position = ModifiedPos;
                t.rotation = ModifiedRot;
                t.localScale = ModifiedScale;
            }
        }

        public Vector3 GetNormal()
        {
            return ModifiedRot * Vector3.right;
        }

        #endregion
    }
}