A netstandard 2.0 port of http://emitmapper.codeplex.com/

## Project Description
Powerful customisable tool for mapping entities to each other. Entities can be plain objects, DataReaders, SQL commands and anything you need. The tool uses run-time code generation via the Emit library. It is usefull for dealing with DTO objects, data access layers an so on.

# About Emit Mapper

* Overview
* Benefits of Emit Mapper
* Getting started
* Type conversion
* Customization

# Customization overview

Customization using default configurator
* Default configurator overview
* Custom converters
* Custom converters_for_generics
* Null substitution
* Ignoring members
* Custom constructors
* Shallow and_deep_mapping
* Names matching
* Post processing

Low-level customization using custom configuratorors

#Benchmarks

```
BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i5-4670 CPU 3.40GHz (Haswell), 1 CPU, 4 logical and 4 physical cores
Frequency=3320317 Hz, Resolution=301.1761 ns, Timer=TSC
.NET Core SDK=2.1.300
  [Host] : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Core   : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT

Job=Core  Runtime=Core

                    Method |           Mean |       Error |      StdDev |     Gen 0 |    Gen 1 |  Allocated |
-------------------------- |---------------:|------------:|------------:|----------:|---------:|-----------:|
    EmitMapper_BenchSource |      0.6332 ns |   0.0123 ns |   0.0115 ns |    0.9584 |        - |     3016 B |
    AutoMapper_BenchSource |     12.6476 ns |   0.0695 ns |   0.0650 ns |    1.5259 |        - |     4840 B |
         EmitMapper_Simple |      0.0512 ns |   0.0006 ns |   0.0005 ns |    0.0380 |        - |      120 B |
         AutoMapper_Simple |     37.8642 ns |   0.2814 ns |   0.2633 ns |    4.2114 |        - |    13399 B |
  EmitMapper_SimpleList100 |      7.8069 ns |   0.0136 ns |   0.0127 ns |    4.5319 |        - |    14304 B |
  AutoMapper_SimpleList100 |  3,838.4431 ns |  32.4725 ns |  30.3748 ns |  421.8750 |        - |  1338924 B |
 EmitMapper_SimpleList1000 |     87.3248 ns |   1.6981 ns |   1.7438 ns |   38.2080 |   7.3242 |   136712 B |
 AutoMapper_SimpleList1000 | 38,264.8686 ns | 261.5550 ns | 244.6587 ns | 4125.0000 | 187.5000 | 13383313 B |
```
