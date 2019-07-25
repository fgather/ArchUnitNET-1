/*
 * Copyright 2019 Florian Gather <florian.gather@tngtech.com>
 * Copyright 2019 Paula Ruiz <paula.ruiz@tngtech.com>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using Equ;

namespace ArchUnitNET.Domain.Dependencies.Types
{
    public class InheritsBaseClassDependency : MemberwiseEquatable<InheritsBaseClassDependency>, ITypeDependency
    {
        public InheritsBaseClassDependency(Class origin, Class target)
        {
            Origin = origin;
            Target = target;
        }

        public IType Origin { get; }
        public IType Target { get; }
    }
}