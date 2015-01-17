//
//  ProxyObject.cs
//
//  AKiOS Universal Unity Plugin for iOS, https://github.com/alex-kir/AKiOS
//
//  Created by Alexander Kirienko on 03.04.14.
//  Copyright (c) 2013-2015 Alexander Kirienko. All rights reserved.
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//
using System;
using System.Collections.Generic;

namespace AKiOS.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CocoaSuperClassAttribute : Attribute
    {
        public string Name;

        public CocoaSuperClassAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CocoaProtocolAttribute : Attribute
    {
        public string Name;

        public CocoaProtocolAttribute(string name)
        {
            Name = name;
        }
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class CocoaMethodAttribute : Attribute
    {
        public string Name;

        public CocoaMethodAttribute(string name)
        {
            Name = name;
        }
    }

    public class ProxyObject : AKiOS.NSObject
    {
        protected static readonly Dictionary<IntPtr, object> instances = new Dictionary<IntPtr, object>();

        protected void InitProxyObject(string classMethod, string instanceMethod = null, params object [] methodArgs)
        {
            var className = this.GetType().Name;
            var classObj = Class.FindByName(className);
            if (classObj.IsNil)
            {
                Functions.NSLog("ProxyObject.InitProxyObject() for " + className);

                string superClass = "NSObject";
                foreach (CocoaSuperClassAttribute attr in this.GetType().GetCustomAttributes(typeof(CocoaSuperClassAttribute), true))
                {
                    superClass = attr.Name;
                    break;
                }
                var classImpl = new Class(className, superClass);

                foreach (CocoaProtocolAttribute attr in this.GetType().GetCustomAttributes(typeof(CocoaProtocolAttribute), true))
                {
                    classImpl.AddProtocol(attr.Name);
                }

                var methods = this.GetType().GetMethods(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Static
                );

                foreach (var method_ in methods)
                {
                    var method = method_;
                    foreach (CocoaMethodAttribute attr in method.GetCustomAttributes(typeof(CocoaMethodAttribute), true))
                    {
                        var paramInfos = method.GetParameters();
                        classImpl.AddMethod(attr.Name, args =>
                        {
                            try
                            {
                                object obj = null;
                                instances.TryGetValue(args.GetSelf().Handle, out obj);

                                var paramValues = new object[paramInfos.Length];
                                for (int i = 0; i < paramInfos.Length; i++)
                                {
                                    var info = paramInfos[i];
                                    if (info.ParameterType == typeof(Core.Arguments))
                                    {
                                        paramValues[i] = args;
                                    }
                                    else if (info.ParameterType == typeof(UnknownValue))
                                    {
                                        paramValues[i] = args.GetValue(i);
                                    }
                                    else if (info.ParameterType == typeof(NSObject) || info.ParameterType.IsSubclassOf(typeof(NSObject)))
                                    {
                                        paramValues[i] = args.GetValue(i).Cast(info.ParameterType);
                                    }
                                    else
                                    {
                                        paramValues[i] = info.DefaultValue;
                                    }
                                }

                                var ret = method.Invoke(obj, paramValues);
                                if (ret != null)
                                {
                                    UnknownValue value1;
                                    NSObject value2;
                                    if ((value1 = ret as UnknownValue) != null)
                                    {
                                        args.ReturnValue = value1;
                                    }
                                    else if ((value2 = ret as NSObject) != null)
                                    {
                                        args.ReturnValue = new UnknownValue(value2.Handle);
                                    }
                                    else // if struct
                                    {

                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                Functions.NSLog("ProxyObject.InitProxyObject() EXCEPTION = " + ex);
                            }
                        });
                        break;
                    }
                }
                classImpl.Register();

                classObj = Class.FindByName(className);
            }
            if (instanceMethod == null)
            {
                this.Handle = classObj.Call(classMethod).Handle;
            }
            else
            {
                var instance = classObj.Call(classMethod);
                this.Handle = instance.Call(instanceMethod, methodArgs).Handle;
            }
            instances[this.Handle] = this;
        }
    }
}


