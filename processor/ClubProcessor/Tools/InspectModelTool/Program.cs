// Tools\InspectModelTool\Program.cs
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace InspectModelTool
{
    internal static class Program
    {
        private static int Main()
        {
            try
            {
                // Adjust if you built Release or a different framework
                var assemblyPath = Path.GetFullPath(Path.Combine("..", "ClubProcessor", "bin", "Debug", "net9.0", "ClubProcessor.dll"));
                if (!File.Exists(assemblyPath))
                {
                    Console.WriteLine($"Could not find assembly at: {assemblyPath}");
                    return 2;
                }

                var asm = Assembly.LoadFrom(assemblyPath);

                // Try to find a design-time factory type named EventDbContextFactory
                var factoryType = asm.GetTypes().FirstOrDefault(t => t.Name.Equals("EventDbContextFactory", StringComparison.Ordinal));
                DbContext dbContext = null;

                if (factoryType != null)
                {
                    // Create instance reflectively and call CreateDbContext
                    var factory = Activator.CreateInstance(factoryType);
                    var createMethod = factoryType.GetMethod("CreateDbContext", new[] { typeof(string[]) }) ??
                                       factoryType.GetMethod("CreateDbContext", Type.EmptyTypes);

                    if (createMethod == null)
                    {
                        Console.WriteLine("Found EventDbContextFactory but could not find CreateDbContext method.");
                        return 3;
                    }

                    dbContext = createMethod.GetParameters().Length == 0
                        ? createMethod.Invoke(factory, null) as DbContext
                        : createMethod.Invoke(factory, new object[] { Array.Empty<string>() }) as DbContext;

                    if (dbContext == null)
                    {
                        Console.WriteLine("Failed to create DbContext instance from factory.");
                        return 4;
                    }
                }
                else
                {
                    // Fallback: find EventDbContext type and try to construct with parameterless ctor
                    var ctxType = asm.GetTypes().FirstOrDefault(t => t.Name.Equals("EventDbContext", StringComparison.Ordinal) && typeof(DbContext).IsAssignableFrom(t));
                    if (ctxType == null)
                    {
                        Console.WriteLine("Neither EventDbContextFactory nor EventDbContext were found in the assembly.");
                        return 5;
                    }

                    var ctor = ctxType.GetConstructor(Type.EmptyTypes);
                    if (ctor == null)
                    {
                        Console.WriteLine("EventDbContext found but no parameterless constructor. Use factory or adjust ctor for inspection.");
                        return 6;
                    }

                    dbContext = Activator.CreateInstance(ctxType) as DbContext;
                    if (dbContext == null)
                    {
                        Console.WriteLine("Created object is not a DbContext");
                        return 7;
                    }
                }

                InspectCompetitorEntity(dbContext);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InspectModel failure: {ex}");
                return 10;
            }
        }

        private static void InspectCompetitorEntity(DbContext db)
        {
            var t = db.Model.GetEntityTypes().FirstOrDefault(e => e.Name.EndsWith("Competitor"));
            if (t == null)
            {
                Console.WriteLine("Competitor entity not found in model");
                return;
            }

            Console.WriteLine("Properties EF knows about for Competitor:");
            foreach (var p in t.GetProperties().OrderBy(p => p.Name))
                Console.WriteLine($"- {p.Name} (ClrType: {p.ClrType.Name}, Nullable: {p.IsNullable})");
        }
    }
}
