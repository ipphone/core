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
 * @see http://sites.google.com/site/sipekvoip
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using Sipek.Common.CallControl;
using System.ComponentModel;
using ContactPoint.Common;


/*! \namespace Common
    \brief Common namespace defines general interfaces to various VoIP functionality

    ...
*/

namespace Sipek.Common
{

    /// <summary>
    /// AbstractFactory is an abstract interface providing interfaces for CallControl module. 
    /// It consists of two parts: factory methods and getter methods. First creates instances, 
    /// later returns instances. 
    /// </summary>
    public interface IAbstractFactory
    {
        /// <summary>
        /// Core object
        /// </summary>
        ICore Core { get; }

        /// <summary>
        /// Factory creator. Creates new instance of timer 
        /// </summary>
        /// <returns>ITimer instance</returns>
        ITimer CreateTimer();

        /// <summary>
        /// State machine factory. Use this method to create your own call state machine class.
        /// </summary>
        /// <returns></returns>
        IStateMachine CreateStateMachine();
    }

    #region Null Pattern
    /// <summary>
    /// Null Factory implementation
    /// </summary>
    internal class NullFactory : IAbstractFactory
    {
        #region AbstractFactory members
        // factory methods
        public ICore Core
        {
            get { return null; }
        }
        public ITimer CreateTimer()
        {
            return new NullTimer();
        }
        public IStateMachine CreateStateMachine()
        {
            return new CStateMachine();
        }

        #endregion
    }

    #endregion
}
