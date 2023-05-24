﻿// OnionFruit Web Copyright DragonFruit Network
// Licensed under the MIT License. Please refer to the LICENSE file at the root of this project for details

using System.Collections.Generic;

namespace DragonFruit.OnionFruit.Services.LocationDb.Abstractions
{
    public interface ICountryDatabase : IEnumerable<IDatabaseCountry>
    {
        IDatabaseCountry this[int index] { get; }

        IDatabaseCountry GetCountry(string code);
    }
}
