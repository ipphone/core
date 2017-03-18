/**
 * Tweener.cs
 * Smooth animation helper for .Net controls
 *
 * @author		Peter Strömberg, Projectplace International AB, peter.stromberg@projectplace.com
 * @author		...
 * @version		0.1
 */

/*
Licensed under the MIT License

Copyright (c) 2008 Projectplace International AB

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

http://code.google.com/p/tweener-dotnet/
http://code.google.com/p/tweener-dotnet/wiki/License

Disclaimer for Robert Penner's Easing Equations license:

    TERMS OF USE - EASING EQUATIONS

    Open source under the BSD License.

    Copyright © 2001 Robert Penner
    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

        * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
        * Neither the name of the author nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;

namespace Projectplace.Gui
{
    #region Public utility classes
    /// <summary>
    /// Utility class for convenient listing of property=>destination_value "pairs"
    /// <example>new TweenPairs() {{"X", 200}, {"Opacity", 0.2}}</example>
    /// </summary>
    public class TweenPairs : List<KeyValuePair<string, float>>
    {
        public void Add(string s, float f)
        {
            Add(new KeyValuePair<string, float>(s, f));
        }
    }
    #endregion

    ///<summary>
    /// Statically this class is responsible for managing the life cycle of all registered tweener insances.
    /// Instances of the Tweener class are responsible for animating given properties for a given object.
    ///</summary>
	public class Tweener {

        /// <summary>
        /// It's possible to register functions on the tweener instance to be called when the animation is completed
        /// </summary>
        public delegate void onCompleteFunction();

        /// <summary>
        /// <see cref="Easing functions"/>
        /// </summary>
        /// <param name="t">Current time (in timer ticks)</param>
        /// <param name="b">Starting value</param>
        /// <param name="d">Duration (in timer ticks)</param>
        /// <param name="c">Delta value</param>
        /// <returns></returns>
        public delegate int ease(float t, float b, float d, float c);


        private static Dictionary<object, Tweener> tweeners = new Dictionary<object, Tweener>();
        private static Timer timer;

        private int counter = 0;
		private  int duration;
        private ease easingFunction;
        private Tweener chainedTweener;
        private int delay = 0;
        private onCompleteFunction onComplete;

        private List<KeyValuePair<string, float>> transitions;
        private TweenData[] tween_data;

		private object animated_object;

        /// <summary>
        /// Some properties are not available for updating directly on the object we're animating.
        /// For those occations we "fake" properties by using a PropertyProxy.
        /// </summary>
        /// <see cref="LocationProxy"/>
        private abstract class PropertyProxy
        {
            protected Control control;

            public PropertyProxy(Control control)
            {
                this.control = control;
            }

            /// <summary>
            /// The proxy objects pretends to be the proxied control when participating in dictionaries and such.
            /// </summary>
            /// <param name="animated_object"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                return control.Equals(((LocationProxy)obj).control);
            }

            /// <see cref="Equals"/>
            public override int GetHashCode()
            {
                return control.GetHashCode();
            }
        }

        /// <summary>
        /// Location.X and Location.Y are not directly updateable on .Net controls.
        /// </summary>
        private class LocationProxy : PropertyProxy
        {
            public LocationProxy(Control control) : base(control) {}

            public float X
            {
                get
                {
                    return control.Location.X;
                }
                set
                {
                    control.Location = new Point((int)value, control.Location.Y);
                }
            }

            public float Y
            {
                get
                {
                    return control.Location.Y;
                }
                set
                {
                    control.Location = new Point(control.Location.X, (int)value);
                }
            }
        }

        /// <summary>
        /// Reflection is used to read and set properties on the animated objects.
        /// </summary>
        private class TweenData
        {
            private object _object;
            private float _start;
            private float _dest;
            private PropertyInfo _prop;

            public TweenData(object o, string property_name, float dest)
            {
                this._object = o;
                this._prop = _object.GetType().GetProperty(property_name);
                this._dest = dest;
                this._start = float.Parse(_prop.GetValue(_object, null).ToString());
            }

            public float start
            {
                get
                {
                    return _start;
                }
            }

            public float dest
            {
                get
                {
                    return _dest;
                }
            }

            public void setProperty(float v)
            {
                _prop.SetValue(_object, v, null);
            }
        }

        /// <summary>
        /// Instances of the Tweener class are responsible for animating given properties for a given object.
        /// </summary>
        /// <param name="animated_object">The object to animate</param>
        /// <param name="transistions">A list of propertynames and their destination values.</param>
        /// <param name="easingFunction">The easing funtion to use for the animation.<see cref="ease"/></param>
        /// <param name="duration">The duration (in timer ticks) the animation should take to complete.</param>
        /// <param name="delay">How long to wait (in timer ticks) before starting the animation.</param>
        public Tweener(object animated_object, TweenPairs transistions, ease easingFunction, int duration, int delay)
        {
            this.duration = duration;
            this.easingFunction = new ease(easingFunction);
            this.delay = delay;
            this.transitions = transistions;
            if (animated_object is Control && (transistions[0].Key == "X" || transistions[0].Key == "Y"))
            {
                animated_object = new LocationProxy((Control)animated_object);
            }
            this.animated_object = animated_object;
        }

        public Tweener(object animated_object, TweenPairs transitions, ease easingFunction, int duration, int delay, Tweener chainedTweener)
            :this(animated_object, transitions, easingFunction, duration, delay)
        {
            this.chainedTweener = chainedTweener;
        }

        public void setOnComplete(onCompleteFunction f)
        {
            onComplete = new onCompleteFunction(f);
        }

        private void init()
        {
            tween_data = new TweenData[transitions.Count];
            for (int i = 0, n = transitions.Count; i < n; i++)
            {
                tween_data[i] = new TweenData(animated_object, transitions[i].Key, transitions[i].Value);
            }
        }

        /// <summary>
        /// Register a tweener that this class should manage.
        /// </summary>
        /// <param name="tweener"></param>
        public static void add(Tweener tweener)
        {
            if (timer == null)
            {
                timer = new Timer();
                timer.Interval = 15;
                timer.Tick += new EventHandler(timer_Tick);
            }
            if (!tweeners.ContainsKey(tweener.animated_object))
            {
                tweener.init();
                tweeners.Add(tweener.animated_object, tweener);
                timer.Enabled = true;
            }
        }

		///<summary>
        ///Each timer tick we update all active tweeners.
        ///When a tweener is finished we remove it, add any chained tweener and call any onComplete function registred.
		///</summary>
		public static void timer_Tick(object sender, System.EventArgs e) {
            List<object> r = new List<object>();
            List<Tweener> a = new List<Tweener>();

            foreach (Tweener tweener in tweeners.Values)
            {
                tweener.animate();
                if (tweener.finished())
                {
                    if (tweener.onComplete != null)
                    {
                        tweener.onComplete();
                    }
                    r.Add(tweener.animated_object);
                    if (tweener.chainedTweener != null)
                    {
                        a.Add(tweener.chainedTweener);
                    }
                }
            }

            foreach (object o in r)
            {
                tweeners.Remove(o);
            }
            foreach (Tweener t in a)
            {
                add(t);
            }
            if (tweeners.Keys.Count == 0)
            {
                timer.Enabled = false;
            }
        }

        private bool finished()
        {
            return counter >= duration;
        }

        /// <summary>
        /// This timer tick's fraction of the transition of the animated object takes place here.
        /// </summary>
        private void animate()
        {
            if (delay == 0)
            {
                for (int i = 0, n = tween_data.Length; i < n; i++)
                {
                    tween_data[i].setProperty(tween(tween_data[i].start, tween_data[i].dest));
                }
                counter++;
            }
            else
            {
                delay--;
            }
        }

        ///<summary>
		///Fetch a value from the easing formula.
		///</summary>
		private int	tween(float start, float dest)
        {
			float t = (float)counter;
			float b = start;
			float c = dest - start;
			float d = (float)duration;
			return easingFunction(t, b, d, c);
        }

        #region Easing functions
        // Most (all?) of the equations used here are Robert Penner's work as mentioned on the disclaimer.

        /// <summary>
        /// quadratic (t^2) easingFunction in - accelerating from zero velocity
        /// </summary>
        /// <see cref="ease"/>
        public static int easeInQuad(float t, float b, float d, float c)
        {
            return (int)(c * (t /= d) * t + b);
        }

        /// <summary>
        /// quadratic (t^2) easingFunction out - deccelerating to zero velocity
        /// </summary>
        /// <see cref="ease"/>
        public static int easeOutQuad(float t, float b, float d, float c) {
            return (int)(-c * (t = t / d) * (t - 2) + b);
        }

        /// <summary>
        /// quadratic easingFunction in/out - acceleration until halfway, then deceleration
        /// </summary>
        /// <see cref="ease"/>
        public static int easeInOutQuad(float t, float b, float d, float c)
        {
			if ((t/=d/2)<1) return (int)(c/2*t*t+b); else return (int)(-c/2*((--t)*(t-2)-1)+b);
        }

        /// <summary>
        /// cubic easingFunction in - accelerating from zero velocity
        /// </summary>
        /// <see cref="ease"/>
        public static int easeInCubic(float t, float b, float d, float c)
        {
			return (int)(c*(t/=d)*t*t + b);
        }

        /// <summary>
        /// cubic easingFunction out - deccelerating to zero velocity
        /// </summary>
        /// <see cref="ease"/>
        public static int easeOutCubic(float t, float b, float d, float c)
        {
			return (int)(c*((t=t/d-1)*t*t + 1) + b);						
        }

        /// <summary>
        /// cubic easingFunction in/out - acceleration until halfway, then deceleration
        /// </summary>
        /// <see cref="ease"/>
        public static int easeInOutCubic(float t, float b, float d, float c)
        {
			if ((t/=d/2) < 1)return (int)(c/2*t*t*t+b);else return (int)(c/2*((t-=2)*t*t + 2)+b);
        }

        /// <summary>
        /// quartic easingFunction in - accelerating from zero velocity
        /// </summary>
        /// <see cref="ease"/>
        public static int easeInQuart(float t, float b, float d, float c)
        {
			return (int)(c*(t/=d)*t*t*t + b);
        }

        /// <summary>
        /// exponential (2^t) easingFunction in - accelerating from zero velocity
        /// </summary>
        /// <see cref="ease"/>
        public static int easeInExpo(float t, float b, float d, float c)
        {
			if (t==0) return (int)b; else return (int)(c*Math.Pow(2,(10*(t/d-1)))+b);
        }

        /// <summary>
        /// exponential (2^t) easingFunction out - decelerating to zero velocity
        /// </summary>
        /// <see cref="ease"/>
        public static int easeOutExpo(float t, float b, float d, float c)
        {
			if (t==d) return (int)(b+c); else return (int)(c * (-Math.Pow(2,-10*t/d)+1)+b);
        }

        /// <summary>
        /// easingFunction past the destination then back - deccelerating to zero
        /// </summary>
        /// <see cref="ease"/>
        public static int easeOutBack(float t, float b, float d, float c)
        {
            float s = 1.25F; // 1.70158F;
            return (int)(c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b);
        }

        /// <summary>
        /// linear tweening (no easingFunction)
        /// </summary>
        /// <see cref="ease"/>
        public static int linear(float t, float b, float d, float c)
        {
            return (int)(c * t / d + b);
        }
        #endregion
    }
}
