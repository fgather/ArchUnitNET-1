﻿using System;
using System.Collections.Generic;
using System.Linq;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent.Extensions;
using static ArchUnitNET.Domain.Visibility;

namespace ArchUnitNET.Fluent.Syntax.Elements
{
    public static class ObjectsFilterDefinition<T> where T : ICanBeAnalyzed
    {
        public static ObjectFilter<T> Are(ICanBeAnalyzed firstObject, params ICanBeAnalyzed[] moreObjects)
        {
            var description = moreObjects.Aggregate("are \"" + firstObject.FullName + "\"",
                (current, obj) => current + " or \"" + obj.FullName + "\"");
            return new ObjectFilter<T>(o => o.Equals(firstObject) || moreObjects.Any(o.Equals), description);
        }

        public static ObjectFilter<T> DependOn(string pattern)
        {
            return new ObjectFilter<T>(obj => obj.DependsOn(pattern), "depend on \"" + pattern + "\"");
        }

        public static ObjectFilter<T> DependOn(IType firstType, params IType[] moreTypes)
        {
            bool Condition(T type)
            {
                return type.Dependencies.Select(dependency => dependency.Target)
                    .Any(target => target.Equals(firstType) || moreTypes.Contains(target));
            }

            var description = moreTypes.Aggregate("depend on \"" + firstType.FullName + "\"",
                (current, obj) => current + " or \"" + obj.FullName + "\"");
            return new ObjectFilter<T>(Condition, description);
        }

        public static ArchitectureObjectFilter<T> DependOn(Type firstType, params Type[] moreTypes)
        {
            bool Condition(T type, Architecture architecture)
            {
                return type.Dependencies.Select(dependency => dependency.Target)
                    .Any(target =>
                        target.Equals(architecture.GetTypeOfType(firstType)) ||
                        moreTypes.Any(t => architecture.GetTypeOfType(t).Equals(target)));
            }

            var description = moreTypes.Aggregate("depend on \"" + firstType.FullName + "\"",
                (current, obj) => current + " or \"" + obj.FullName + "\"");
            return new ArchitectureObjectFilter<T>(Condition, description);
        }

        public static ArchitectureObjectFilter<T> DependOn(IObjectProvider<IType> objectProvider)
        {
            bool Condition(T type, Architecture architecture)
            {
                var types = objectProvider.GetObjects(architecture);
                return type.Dependencies.Select(dependency => dependency.Target)
                    .Any(target => types.Contains(target));
            }

            var description = "depend on " + objectProvider.Description;
            return new ArchitectureObjectFilter<T>(Condition, description);
        }

        public static ObjectFilter<T> DependOn(IEnumerable<IType> types)
        {
            var typeList = types.ToList();

            bool Condition(T type)
            {
                return type.Dependencies.Select(dependency => dependency.Target)
                    .Any(target => typeList.Any(t => t.Equals(target)));
            }

            string description;
            if (typeList.IsNullOrEmpty())
            {
                description = "have no dependencies";
            }
            else
            {
                var firstType = typeList.First();
                description = typeList.Where(obj => !obj.Equals(firstType)).Distinct().Aggregate(
                    "depend on \"" + firstType.FullName + "\"",
                    (current, obj) => current + " or \"" + obj.FullName + "\"");
            }

            return new ObjectFilter<T>(Condition, description);
        }

        public static ObjectFilter<T> OnlyDependOn(string pattern)
        {
            return new ObjectFilter<T>(obj => obj.OnlyDependsOn(pattern), "only depend on \"" + pattern + "\"");
        }

        public static ObjectFilter<T> OnlyDependOn(IType firstType, params IType[] moreTypes)
        {
            bool Condition(T type)
            {
                return type.Dependencies.Select(dependency => dependency.Target)
                    .All(target => target.Equals(firstType) || moreTypes.Contains(target));
            }

            var description = moreTypes.Aggregate("only depend on \"" + firstType.FullName + "\"",
                (current, obj) => current + " or \"" + obj.FullName + "\"");
            return new ObjectFilter<T>(Condition, description);
        }

        public static ArchitectureObjectFilter<T> OnlyDependOn(Type firstType, params Type[] moreTypes)
        {
            bool Condition(T type, Architecture architecture)
            {
                return type.Dependencies.Select(dependency => dependency.Target)
                    .All(target =>
                        target.Equals(architecture.GetTypeOfType(firstType)) ||
                        moreTypes.Any(t => architecture.GetTypeOfType(t).Equals(target)));
            }

            var description = moreTypes.Aggregate("only depend on \"" + firstType.FullName + "\"",
                (current, obj) => current + " or \"" + obj.FullName + "\"");
            return new ArchitectureObjectFilter<T>(Condition, description);
        }

        public static ArchitectureObjectFilter<T> OnlyDependOn(IObjectProvider<IType> objectProvider)
        {
            bool Condition(T type, Architecture architecture)
            {
                var types = objectProvider.GetObjects(architecture);
                return type.Dependencies.Select(dependency => dependency.Target)
                    .All(target => types.Contains(target));
            }

            var description = "only depend on " + objectProvider.Description;
            return new ArchitectureObjectFilter<T>(Condition, description);
        }

        public static ObjectFilter<T> OnlyDependOn(IEnumerable<IType> types)
        {
            var typeList = types.ToList();

            bool Condition(T type)
            {
                return type.Dependencies.Select(dependency => dependency.Target)
                    .All(target => typeList.Contains(target));
            }

            string description;
            if (typeList.IsNullOrEmpty())
            {
                description = "have no dependencies";
            }
            else
            {
                var firstType = typeList.First();
                description = typeList.Where(obj => !obj.Equals(firstType)).Distinct().Aggregate(
                    "only depend on \"" + firstType.FullName + "\"",
                    (current, obj) => current + " or \"" + obj.FullName + "\"");
            }

            return new ObjectFilter<T>(Condition, description);
        }

        public static ObjectFilter<T> HaveName(string name)
        {
            return new ObjectFilter<T>(obj => obj.Name.Equals(name), "have name \"" + name + "\"");
        }

        public static ObjectFilter<T> HaveFullName(string fullname)
        {
            return new ObjectFilter<T>(obj => obj.FullName.Equals(fullname), "have full name \"" + fullname + "\"");
        }

        public static ObjectFilter<T> HaveNameStartingWith(string pattern)
        {
            return new ObjectFilter<T>(obj => obj.NameStartsWith(pattern),
                "have name starting with \"" + pattern + "\"");
        }

        public static ObjectFilter<T> HaveNameEndingWith(string pattern)
        {
            return new ObjectFilter<T>(obj => obj.NameEndsWith(pattern), "have name ending with \"" + pattern + "\"");
        }

        public static ObjectFilter<T> HaveNameContaining(string pattern)
        {
            return new ObjectFilter<T>(obj => obj.NameContains(pattern), "have name containing \"" + pattern + "\"");
        }

        public static ObjectFilter<T> ArePrivate()
        {
            return new ObjectFilter<T>(obj => obj.Visibility == Private, "are private");
        }

        public static ObjectFilter<T> ArePublic()
        {
            return new ObjectFilter<T>(obj => obj.Visibility == Public, "are public");
        }

        public static ObjectFilter<T> AreProtected()
        {
            return new ObjectFilter<T>(obj => obj.Visibility == Protected, "are protected");
        }

        public static ObjectFilter<T> AreInternal()
        {
            return new ObjectFilter<T>(obj => obj.Visibility == Internal, "are internal");
        }

        public static ObjectFilter<T> AreProtectedInternal()
        {
            return new ObjectFilter<T>(obj => obj.Visibility == ProtectedInternal, "are protected internal");
        }

        public static ObjectFilter<T> ArePrivateProtected()
        {
            return new ObjectFilter<T>(obj => obj.Visibility == PrivateProtected, "are private protected");
        }


        //Negations


        public static ObjectFilter<T> AreNot(ICanBeAnalyzed firstObject, params ICanBeAnalyzed[] moreObjects)
        {
            var description = moreObjects.Aggregate("are not \"" + firstObject.FullName + "\"",
                (current, obj) => current + " or \"" + obj.FullName + "\"");
            return new ObjectFilter<T>(o => !o.Equals(firstObject) && !moreObjects.Any(o.Equals), description);
        }

        public static ObjectFilter<T> DoNotDependOn(string pattern)
        {
            return new ObjectFilter<T>(obj => !obj.DependsOn(pattern), "do not depend on \"" + pattern + "\"");
        }

        public static ObjectFilter<T> DoNotDependOn(IType firstType, params IType[] moreTypes)
        {
            bool Condition(T type)
            {
                return type.Dependencies.Select(dependency => dependency.Target)
                    .All(target => !target.Equals(firstType) && !moreTypes.Contains(target));
            }

            var description = moreTypes.Aggregate("do not depend on \"" + firstType.FullName + "\"",
                (current, obj) => current + " or \"" + obj.FullName + "\"");
            return new ObjectFilter<T>(Condition, description);
        }

        public static ArchitectureObjectFilter<T> DoNotDependOn(Type firstType, params Type[] moreTypes)
        {
            bool Condition(T type, Architecture architecture)
            {
                return type.Dependencies.Select(dependency => dependency.Target)
                    .All(target =>
                        !target.Equals(architecture.GetTypeOfType(firstType)) &&
                        moreTypes.All(t => !architecture.GetTypeOfType(t).Equals(target)));
            }

            var description = moreTypes.Aggregate("do not depend on \"" + firstType.FullName + "\"",
                (current, obj) => current + " or \"" + obj.FullName + "\"");
            return new ArchitectureObjectFilter<T>(Condition, description);
        }

        public static ArchitectureObjectFilter<T> DoNotDependOn(IObjectProvider<IType> objectProvider)
        {
            bool Condition(T type, Architecture architecture)
            {
                var types = objectProvider.GetObjects(architecture);
                return type.Dependencies.Select(dependency => dependency.Target)
                    .All(target => types.All(t => !t.Equals(target)));
            }

            var description = "do not depend on " + objectProvider.Description;
            return new ArchitectureObjectFilter<T>(Condition, description);
        }

        public static ObjectFilter<T> DoNotDependOn(IEnumerable<IType> types)
        {
            var typeList = types.ToList();

            bool Condition(T type)
            {
                return type.Dependencies.Select(dependency => dependency.Target)
                    .All(target => typeList.All(t => !t.Equals(target)));
            }

            string description;
            if (typeList.IsNullOrEmpty())
            {
                description = "do not depend on no types (always true)";
            }
            else
            {
                var firstType = typeList.First();
                description = typeList.Where(obj => !obj.Equals(firstType)).Distinct().Aggregate(
                    "do not depend on \"" + firstType.FullName + "\"",
                    (current, obj) => current + " or \"" + obj.FullName + "\"");
            }

            return new ObjectFilter<T>(Condition, description);
        }

        public static ObjectFilter<T> DoNotHaveName(string name)
        {
            return new ObjectFilter<T>(obj => !obj.Name.Equals(name), "do not have name \"" + name + "\"");
        }

        public static ObjectFilter<T> DoNotHaveFullName(string fullname)
        {
            return new ObjectFilter<T>(obj => !obj.FullName.Equals(fullname),
                "do not have full name \"" + fullname + "\"");
        }

        public static ObjectFilter<T> DoNotHaveNameStartingWith(string pattern)
        {
            return new ObjectFilter<T>(obj => !obj.NameStartsWith(pattern),
                "do not have name starting with \"" + pattern + "\"");
        }

        public static ObjectFilter<T> DoNotHaveNameEndingWith(string pattern)
        {
            return new ObjectFilter<T>(obj => !obj.NameEndsWith(pattern),
                "do not have name ending with \"" + pattern + "\"");
        }

        public static ObjectFilter<T> DoNotHaveNameContaining(string pattern)
        {
            return new ObjectFilter<T>(obj => !obj.NameContains(pattern),
                "do not have name containing \"" + pattern + "\"");
        }

        public static ObjectFilter<T> AreNotPrivate()
        {
            return new ObjectFilter<T>(obj => obj.Visibility != Private, "are not private");
        }

        public static ObjectFilter<T> AreNotPublic()
        {
            return new ObjectFilter<T>(obj => obj.Visibility != Public, "are not public");
        }

        public static ObjectFilter<T> AreNotProtected()
        {
            return new ObjectFilter<T>(obj => obj.Visibility != Protected, "are not protected");
        }

        public static ObjectFilter<T> AreNotInternal()
        {
            return new ObjectFilter<T>(obj => obj.Visibility != Internal, "are not internal");
        }

        public static ObjectFilter<T> AreNotProtectedInternal()
        {
            return new ObjectFilter<T>(obj => obj.Visibility != ProtectedInternal, "are not protected internal");
        }

        public static ObjectFilter<T> AreNotPrivateProtected()
        {
            return new ObjectFilter<T>(obj => obj.Visibility != PrivateProtected, "are not private protected");
        }
    }
}