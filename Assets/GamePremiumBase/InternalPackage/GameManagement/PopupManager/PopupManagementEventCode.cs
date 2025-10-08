using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Premium.PopupManagement
{
    [EventCode]
    public enum PopupManagementEventCode
    {
        /// <summary>
        /// This event is raised when a popup is called to show
        /// <para> <typeparamref name="ShowPopupRequest"/>: request </para>
        /// </summary>
        OnPopupShowed,
        /// <summary>
        /// This event is raised when a popup is called to hide
        /// <para> <typeparamref name="ShowPopupRequest"/>: request </para>
        /// </summary>
        OnPopupClosed,
        /// <summary>
        /// This event is raised when the first popup is called to show
        /// <para> <typeparamref name="ShowPopupRequest"/>: request </para>
        /// </summary>
        OnFirstPopupShowed,
        /// <summary>
        /// This event is raised when the last popup is called to hide
        /// <para> <typeparamref name="ShowPopupRequest"/>: request </para>
        /// </summary>
        OnLastPopupClosed,
    }
}