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
 * 
 * @see http://sipekphone.googlepages.com/pjsipwrapper
 * @see http://voipengine.googlepages.com/
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Sipek.Common
{

    /// <summary>
    /// Tone modes
    /// </summary>
    public enum ETones : int
    {
        EToneDial = 0,
        EToneCongestion,
        EToneRingback,
        EToneRing,
    }

    /// <summary>
    /// Media proxy interface for playing tones (todo recording)
    /// </summary>
    public interface IMediaProxyInterface
    {
        /// <summary>
        /// Session ID to use with call;
        /// </summary>
        int SessionId { get; set; }

        /// <summary>
        /// Play give tone 
        /// </summary>
        /// <param name="toneId">tone identification</param>
        /// <returns></returns>
        int playTone(ETones toneId);

        /// <summary>
        /// Stop tone
        /// </summary>
        /// <returns></returns>
        int stopTone();
    }

    #region Null Pattern

    internal class NullMediaProxy : IMediaProxyInterface
    {
        #region IMediaProxyInterface Members

        public int SessionId { get; set; }

        public int playTone(ETones toneId)
        {
            return 1;
        }

        public int stopTone()
        {
            return 1;
        }
        #endregion
    }
    #endregion


}
