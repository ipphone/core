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
 * @see http://voipengine.googlepages.com/
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Sipek.Common
{
  /// <summary>
  /// Call modes
  /// </summary>
  public enum ECallType : int
  {
    EDialed,
    EReceived,
    EMissed,
    EAll,
    EUndefined
  }

  /// <summary>
  /// Container providing call log functionality
  /// </summary>
  public interface ICallLogInterface
  {
    /// <summary>
    /// Add call to call log
    /// </summary>
    /// <param name="type">call mode</param>
    /// <param name="number">calling/called number</param>
    /// <param name="name">calling/called name</param>
    /// <param name="time">time of call</param>
    /// <param name="duration">duration of call</param>
    void addCall(ECallType type, string number, string name, System.DateTime time, System.TimeSpan duration);

    /// <summary>
    /// Save call log records
    /// </summary>
    void save();

    /// <summary>
    /// get list of logged calls
    /// </summary>
    /// <returns>Call log list</returns>
    Stack<CCallRecord> getList();

    /// <summary>
    /// get list of logged calls by call mode
    /// </summary>
    /// <param name="type">call mode</param>
    /// <returns>Call log list</returns>
    Stack<CCallRecord> getList(ECallType type);

    /// <summary>
    /// Delete single record
    /// </summary>
    /// <param name="record">record to delete</param>
    void deleteRecord(CCallRecord record);
  }



  /// <summary>
  /// Call record
  /// </summary>
  public class CCallRecord
  {
    private ECallType _type;
    private string _name = "";
    private string _number = "";
    private DateTime _time;
    private TimeSpan _duration;
    private int _count;

    /// <summary>
    /// Call name
    /// </summary>
    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }
    /// <summary>
    /// Call number
    /// </summary>
    public string Number
    {
      get { return _number; }
      set { _number = value; }
    }
    /// <summary>
    /// Call mode
    /// </summary>
    public ECallType Type
    {
      get { return _type; }
      set { _type = value; }
    }
    /// <summary>
    /// Duration of call
    /// </summary>
    public TimeSpan Duration
    {
      get { return _duration; }
      set { _duration = value; }
    }
    /// <summary>
    /// Call time
    /// </summary>
    public DateTime Time
    {
      get { return _time; }
      set { _time = value; }
    }
    /// <summary>
    /// Number of records  
    /// </summary>
    public int Count
    {
      get { return _count; }
      set { _count = value; }
    }
  }

  #region Null Pattern

  public class NullCallLogger : ICallLogInterface
  {
    public void addCall(ECallType type, string number, string name, System.DateTime time, System.TimeSpan duration) { }

    public void save() { }
    public Stack<CCallRecord> getList() { return null; }
    public Stack<CCallRecord> getList(ECallType type) { return null; }
    public void deleteRecord(CCallRecord record) { }
  }
  #endregion
}
