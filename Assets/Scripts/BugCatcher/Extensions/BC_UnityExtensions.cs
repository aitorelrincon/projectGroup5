using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BugCatcher.Extensions
{
    public static class BC_UnityExtensions
    {
        /// <summary>
        /// Returns true when the passed GameObject is "Missing"
        /// or "None" (Unassigned).
        /// </summary>
        /// <param name="go">UnityEngine.GameObject instance</param>
        /// <returns>True when the passed GameObject is "Missing"
        /// or "None" (Unassigned).</returns>
        public static bool IsMissingOrNone( this GameObject go )
            => go is null || !go;

        /// <summary>
        /// Returns true when the passed Component is "Missing"
        /// or "None" (Unassigned).
        /// </summary>
        /// <param name="go">UnityEngine.Component instance</param>
        /// <returns>True when the passed Component is "Missing"
        /// or "None" (Unassigned).</returns>
        public static bool IsMissingOrNone( this Component go )
            => go is null || !go;

        /// <summary>
        /// Sets the current Animation to "name". 
        /// This method assumes all parameters are bools.
        /// </summary>
        /// <param name="animator">Animator Component</param>
        /// <param name="name">Animation name</param>
        public static void SetAnimation( this Animator animator, string name )
        {
            AnimatorControllerParameter[] parametros = animator.parameters;

            foreach ( var item in parametros ) animator.SetBool( item.name, false );

            animator.SetBool( name, true );
        }
    }
}