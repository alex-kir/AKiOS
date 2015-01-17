//
//  AKiOS_Core.m
//
//  AKiOS Universal Unity Plugin for iOS, https://github.com/alex-kir/AKiOS
//
//  Created by Alexander Kirienko on 01.07.13.
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

#import "AKiOS_Core.h"

#import <Foundation/Foundation.h>
#import <objc/runtime.h>

typedef struct MonoDomain_s MonoDomain;
typedef struct MonoAssembly_s MonoAssembly;
typedef struct MonoImage_s MonoImage;
typedef struct MonoClass_s MonoClass;
typedef struct MonoObject_s MonoObject;
typedef struct MonoMethodDesc_s MonoMethodDesc;
typedef struct MonoMethod_s MonoMethod;
typedef struct MonoString_s MonoString;
typedef int    gboolean;
typedef void * gpointer;


MonoDomain*     mono_domain_get();
MonoAssembly*   mono_domain_assembly_open(MonoDomain* domain, const char *assemblyName);
MonoImage*      mono_assembly_get_image(MonoAssembly* assembly);
MonoMethodDesc* mono_method_desc_new(const char* methodString, gboolean useNamespace);
MonoMethodDesc* mono_method_desc_free(MonoMethodDesc* desc);
MonoMethod*     mono_method_desc_search_in_image(MonoMethodDesc* methodDesc, MonoImage* image);
MonoObject*     mono_runtime_invoke(MonoMethod* method, void* obj, void** params, MonoObject** exc);
MonoClass*      mono_class_from_name (MonoImage *image, const char* name_space, const char *name);
MonoString*     mono_string_new(MonoDomain* domain, const char* str);
gpointer        mono_object_unbox(MonoObject *obj);


static MonoMethod * _CallbackMethod = 0;
static void * _returnedBytes = 0;

void * AKiOS_Core_CreateClass(const char * szClassName, const char * szSuperClassName)
{
    NSString * sSuperClassName = [[NSString alloc] initWithCString:szSuperClassName encoding:NSUTF8StringEncoding];
//    NSString * sClassName = [[NSString alloc] initWithCString:szClassName encoding:NSUTF8StringEncoding];
    
    Class aSuperClass = NSClassFromString(sSuperClassName);
    Class aClass = objc_allocateClassPair(aSuperClass, szClassName/*[sClassName UTF8String]*/, 0);
    
    return (__bridge void*)aClass;
}

void AKiOS_Core_AddProtocol(void * pClass, const char * szProtocolName)
{
    Class aClass = (__bridge Class)pClass;
    NSString * sProtocolName = [[NSString alloc] initWithCString:szProtocolName encoding:NSUTF8StringEncoding];
    Protocol * protocol = NSProtocolFromString(sProtocolName);
    class_addProtocol(aClass, protocol);
}

MonoMethod * _AKiOS_Core_GetCallbackMethod()
{
    if (_CallbackMethod == 0)
    {
//        NSString * assemblyPath = [[[NSBundle mainBundle] bundlePath] stringByAppendingPathComponent:@"Data/Managed/Assembly-CSharp-firstpass.dll"];
        NSString * assemblyPath = [[[NSBundle mainBundle] bundlePath] stringByAppendingPathComponent:@"Data/Managed/AKiOS.dll"];
        
        MonoDomain * domain = mono_domain_get();
        MonoAssembly * monoAssembly = mono_domain_assembly_open(domain, assemblyPath.UTF8String);
        MonoImage * monoImage = mono_assembly_get_image(monoAssembly);
        MonoMethodDesc * desc = mono_method_desc_new("AKiOS.Class:CallbackFunction(int)", true);
        _CallbackMethod = mono_method_desc_search_in_image(desc, monoImage);
        mono_method_desc_free(desc);
    }
    return _CallbackMethod;
}

id _AKiOS_Core_MethodImplementation(id self_, SEL cmd_, ...)
{
    void * arguments[10];
    arguments[0] = (__bridge void*)self_;
    arguments[1] = (void *)sel_getName(cmd_);
    
    NSLog(@"_AKiOS_Core_MethodImplementation(%p, %s)", arguments[0], arguments[1]);
    
    va_list vaArgs;
    va_start(vaArgs, cmd_);
    for (int i = 2; i < sizeof(arguments) / sizeof(arguments[0]); i++)
    {
        arguments[i] = va_arg(vaArgs, void*);
    }
    va_end(vaArgs);

    void * pArguments = arguments;
    void * monoArgs[1];
    monoArgs[0] = &pArguments;
    
    MonoObject * resultObject = mono_runtime_invoke(_AKiOS_Core_GetCallbackMethod(), NULL, monoArgs, NULL);
    
    void * resultPointer = *(void**)mono_object_unbox(resultObject);
    id resultId = (__bridge id)resultPointer;
    return resultId;
}

void AKiOS_Core_AddMethod(void * pClass, const char * szMethodName, const char * szTypes)
{
    Class aClass = (__bridge Class)pClass;
    NSString * sMethodName = [[NSString alloc] initWithCString:szMethodName encoding:NSUTF8StringEncoding];
    SEL aSelector = NSSelectorFromString(sMethodName);
    
    class_addMethod(aClass, aSelector, _AKiOS_Core_MethodImplementation, szTypes ? szTypes : "@@:*");
}

void AKiOS_Core_RegisterClass(void * pClass)
{
    Class aClass = (__bridge Class)pClass;
    objc_registerClassPair(aClass);
}

void * AKiOS_Core_GetClass(const char * szClassName)
{
    NSString * sClassName = [[NSString alloc] initWithCString:szClassName encoding:NSUTF8StringEncoding];
    return (__bridge void*)NSClassFromString(sClassName);
}

//void * AKiOS_Core_NewInstance(void * pClass)
//{
//    Class aClass = (__bridge Class)pClass;
//    id anInstance = [aClass new];
//    return (__bridge void*)anInstance;
//}

void * AKiOS_Core_CallMethod(void * pInstance, const char * szMethodName, void ** ppArgs, int argsCount, void ** ppReturnedBytes)
{
    NSLog(@"AKiOS_Core_CallMethod(%p, %s, %d)", pInstance, szMethodName, argsCount);
    
    NSString * sMethodName = [[NSString alloc] initWithCString:szMethodName encoding:NSUTF8StringEncoding];
    SEL aSelector = NSSelectorFromString(sMethodName);
    if (!aSelector)
    {
        NSLog(@"AKiOS_Core_CallMethod(): aSelector is NULL, %s", szMethodName);
        return 0;
    }
    
    id anInstance = (__bridge id)pInstance;
    if (!anInstance)
    {
        NSLog(@"AKiOS_Core_CallMethod(): anInstance is NULL, %s", szMethodName);
        return 0;
    }
    
    NSMethodSignature * aSignature = [anInstance methodSignatureForSelector:aSelector];
    if (!aSignature)
    {
        NSLog(@"AKiOS_Core_CallMethod(): aSignature is NULL, %s", szMethodName);
        return 0;
    }
    
    NSInvocation * invocation = [NSInvocation invocationWithMethodSignature:aSignature];
    invocation.target = anInstance;
    invocation.selector = aSelector;
    if (argsCount != aSignature.numberOfArguments - 2)
    {
        NSLog(@"(warning) AKiOS_Core_CallMethod(): argsCount(%d) != aSignature.numberOfArguments(%d)", argsCount, (int)aSignature.numberOfArguments - 2);
    }
        
    for (int i = 0; i < argsCount; i++)
    {
        [invocation setArgument:ppArgs[i] atIndex:2 + i];
    }

    [invocation invoke];


    
    NSString * returnType = [NSString stringWithUTF8String:[aSignature methodReturnType]];
//    NSLog(@"AKiOS_Core_CallMethod(): [aSignature methodReturnType] = %@", returnType);
//    NSLog(@"AKiOS_Core_CallMethod(): [aSignature methodReturnLength] = %d", [aSignature methodReturnLength]);
    // v = ? // void
    // @ = 4 // id
    // c = 1 // bool
    // f = 4 // float
    // d = 8 // double
    // {CGRect={CGPoint=ff}{CGSize=ff}} = 16 // CGRect
    
    if ([returnType isEqualToString:@"v"])
    {
        *ppReturnedBytes = 0;
        return 0;
    }
    else if ([returnType isEqualToString:@"@"])
    {
        void * result = 0;
        [invocation getReturnValue:&result];
        *ppReturnedBytes = 0;
        return result;
    }
    else
    {
        NSInteger len = aSignature.methodReturnLength;
        if (_returnedBytes)
            free(_returnedBytes);
        _returnedBytes = malloc(len);
        [invocation getReturnValue:_returnedBytes];
        *ppReturnedBytes = _returnedBytes;
        return (void *)len;
    }
}









