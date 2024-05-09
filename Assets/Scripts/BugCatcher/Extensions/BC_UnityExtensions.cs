using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        /// <param name="co">UnityEngine.Component instance</param>
        /// <returns>True when the passed Component is "Missing"
        /// or "None" (Unassigned).</returns>
        public static bool IsMissingOrNone( this Component co )
            => co is null || !co;

        /// <summary>
        /// Returns the component if the GameObject has it, otherwise
        /// returns 'other'.
        /// Consider using <seealso cref="GetComponentOrElse{T}(GameObject, Func{T})"/> for
        /// the result function calls.
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="go">GameObject</param>
        /// <param name="other">Default value</param>
        /// <returns>Desired component, or default value</returns>
        public static T GetComponentOr<T>( this GameObject go, [DisallowNull] T other )
            where T : Component
        {
            var c = go.GetComponent<T>();
            return c is not null ? c : other;
        }

        /// <summary>
        /// Returns the component if the Component has it, otherwise
        /// returns 'other'.
        /// Consider using <seealso cref="GetComponentOrElse{T}(Component, Func{T})"/> for
        /// the result function calls.
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="go">Component</param>
        /// <param name="other">Default value</param>
        /// <returns>Desired component, or default value</returns>

        public static T GetComponentOr<T>( this Component co, [DisallowNull] T other )
            where T : Component
        {
            var c = co.GetComponent<T>();
            return c is not null ? c : other;
        }


        /// <summary>
        /// Returns the component if the GameObject has it, otherwise calls f
        /// and returns the result.
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="go">GameObject</param>
        /// <param name="f">Default function</param>
        /// <returns>Desired component or function result</returns>
        public static T GetComponentOrElse<T>( this GameObject go, [DisallowNull] Func<T> f )
        {
            var c = go.GetComponent<T>();
            return c is not null ? c : f();
        }

        /// <summary>
        /// Returns the component if the Component has it, otherwise calls f
        /// and returns the result.
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="co">Component</param>
        /// <param name="f">Default function</param>
        /// <returns>Desired component or function result</returns>
        public static T GetComponentOrElse<T>( this Component co, [DisallowNull] Func<T> f )
        {
            var c = co.GetComponent<T>();
            return c is not null ? c : f();
        }

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