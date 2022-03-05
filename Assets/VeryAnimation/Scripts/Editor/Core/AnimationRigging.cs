#if VERYANIMATION_ANIMATIONRIGGING
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using UnityEditor.Animations.Rigging;

namespace VeryAnimation
{
    public class AnimationRigging
    {
        private VeryAnimationWindow vaw { get { return VeryAnimationWindow.instance; } }
        private VeryAnimation va { get { return VeryAnimation.instance; } }

        public const string AnimationRiggingRigName = "VARig";

        public RigBuilder rigBuilder { get; private set; }
        public Rig rig { get; private set; }
        public VeryAnimationRigBuilder vaRigBuilder { get; private set; }
        public VeryAnimationRig vaRig { get; private set; }

        public bool isValid { get { return vaRigBuilder != null && vaRig != null && rigBuilder != null && rig != null; } }

        public void Initialize()
        {
            Release();

            rigBuilder = vaw.gameObject.GetComponent<RigBuilder>();
            rig = null;
            vaRigBuilder = vaw.gameObject.GetComponent<VeryAnimationRigBuilder>();
            vaRig = GetVeryAnimationRig(vaw.gameObject);
            if (vaRig != null)
                rig = vaRig.GetComponent<Rig>();
        }
        public void Release()
        {
            rigBuilder = null;
            rig = null;
            vaRigBuilder = null;
            vaRig = null;
        }

        public void Enable()
        {
            Disable();

            va.StopRecording();
            {
                Create(vaw.gameObject);

                rigBuilder = vaw.gameObject.GetComponent<RigBuilder>();
                vaRigBuilder = vaw.gameObject.GetComponent<VeryAnimationRigBuilder>();
                vaRig = GetVeryAnimationRig(vaw.gameObject);
                rig = vaRig != null ? vaRig.GetComponent<Rig>() : null;
            }
            va.OnHierarchyWindowChanged();
        }
        public void Disable()
        {
            va.StopRecording();
            {
                Delete(vaw.gameObject);
            }
            va.OnHierarchyWindowChanged();

            Release();
        }
        public static VeryAnimationRig GetVeryAnimationRig(GameObject gameObject)
        {
            return ArrayUtility.Find(gameObject.GetComponentsInChildren<VeryAnimationRig>(true), x => x.name == AnimationRiggingRigName && x.transform.parent == gameObject.transform);
        }
        public static void Create(GameObject gameObject)
        {
            var rigBuilder = gameObject.GetComponent<RigBuilder>();
            if (rigBuilder == null)
            {
                rigBuilder = Undo.AddComponent<RigBuilder>(gameObject);
            }

            var vaRigBuilder = gameObject.GetComponent<VeryAnimationRigBuilder>();
            if (vaRigBuilder == null)
            {
                vaRigBuilder = Undo.AddComponent<VeryAnimationRigBuilder>(gameObject);
            }

            //Must be in order before RigBuilder
            {
                var components = vaRigBuilder.GetComponents<MonoBehaviour>();
                var indexRigBuilder = ArrayUtility.FindIndex(components, x => x.GetType() == typeof(RigBuilder));
                var indexVARigBuilder = ArrayUtility.FindIndex(components, x => x.GetType() == typeof(VeryAnimationRigBuilder));
                if (indexRigBuilder >= 0 && indexVARigBuilder >= 0)
                {
                    for (int i = 0; i < indexVARigBuilder - indexRigBuilder; i++)
                        ComponentUtility.MoveComponentUp(vaRigBuilder);
                }
            }

            var vaRig = GetVeryAnimationRig(gameObject);
            if (vaRig == null)
            {
                var rigObj = new GameObject(AnimationRiggingRigName);
                rigObj.transform.SetParent(gameObject.transform);
                rigObj.transform.localPosition = Vector3.zero;
                rigObj.transform.localRotation = Quaternion.identity;
                rigObj.transform.localScale = Vector3.one;
                Undo.RegisterCreatedObjectUndo(rigObj, "");
                var rig = Undo.AddComponent<Rig>(rigObj);
                vaRig = Undo.AddComponent<VeryAnimationRig>(rigObj);
                Undo.RecordObject(rigBuilder, "");
#if UNITY_2020_1_OR_NEWER
                var rigLayer = new RigLayer(rig);   //version 0.3.2
#else
                var rigLayer = new RigBuilder.RigLayer(rig);    //version 0.2.5
#endif
                rigBuilder.layers.Add(rigLayer);
                Selection.activeGameObject = rigObj;
            }
        }
        public static void Delete(GameObject gameObject)
        {
            var rigBuilder = gameObject.GetComponent<RigBuilder>();
            var vaRigBuilder = gameObject.GetComponent<VeryAnimationRigBuilder>();
            var vaRig = GetVeryAnimationRig(gameObject);
            var rig = vaRig != null ? vaRig.GetComponent<Rig>() : null;

            var index = rigBuilder != null && rig != null ? rigBuilder.layers.FindIndex(x => x.rig == rig) : -1;
            if (rig != null)
            {
                Selection.activeGameObject = rig.gameObject;
                Unsupported.DeleteGameObjectSelection();
                if (rig != null)
                    return;
                rig = null;
            }
            if (vaRigBuilder != null)
            {
                Undo.DestroyObjectImmediate(vaRigBuilder);
                vaRigBuilder = null;
            }
            if (rigBuilder != null)
            {
                if (rigBuilder.layers.Count == 1)
                {
                    Undo.DestroyObjectImmediate(rigBuilder);
                }
                else if (index >= 0 && index < rigBuilder.layers.Count)
                {
                    Undo.RecordObject(rigBuilder, "");
                    rigBuilder.layers.RemoveAt(index);
                }
                rigBuilder = null;
            }
        }
    }
}
#endif
