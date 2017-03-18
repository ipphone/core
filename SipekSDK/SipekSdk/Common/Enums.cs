/* 
 * Copyright (C) 2008 Sasa Coh <sasacoh@gmail.com>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Sipek.Common
{

    #region Enums

    /// <summary>
    /// List of user status modes.
    /// Do NOT change the order of enums. It should be synchronized with 
    /// native stack implementation!!!
    /// </summary>
    public enum EUserStatus : int
    {
        AVAILABLE,
        BUSY,
        OTP,
        IDLE,
        AWAY,
        BRB,
        OFFLINE,
        OPT_MAX
        // add new enum here
    }

    /// <summary>
    /// List of supported service codes.
    /// Do NOT change the order of enums. It should be synchronized with 
    /// native stack implementation!!!
    /// </summary>
    public enum EServiceCodes : int
    {
        SC_CD,
        SC_CFU,
        SC_CFNR,
        SC_DND,
        SC_3PTY,
        SC_CFB
        // add new enum here
    }


    public enum ECallNotification : int
    {
        CN_HOLDCONFIRM
    }

    /// <summary>
    /// Dtmf modes
    /// </summary>
    public enum EDtmfMode : int
    {
        DM_Outband,
        DM_Inband,
        DM_Transparent
    }


    public enum ETransportMode : int
    {
        TM_UDP,
        TM_TCP,
        TM_TLS
    }

    public enum ECallMediaState : int
    {
        /** Call currently has no media */
        PJSUA_CALL_MEDIA_NONE = 0,

        /** The media is active */
        PJSUA_CALL_MEDIA_ACTIVE = 1,

        /** The media is currently put on hold by local endpoint */
        PJSUA_CALL_MEDIA_LOCAL_HOLD = 2,

        /** The media is currently put on hold by remote endpoint */
        PJSUA_CALL_MEDIA_REMOTE_HOLD = 3,

        /** The media has reported error (e.g. ICE negotiation) */
        PJSUA_CALL_MEDIA_ERROR = 4
    }

    #endregion

}
