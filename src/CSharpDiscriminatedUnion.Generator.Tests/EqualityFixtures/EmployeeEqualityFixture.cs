using CSharpDiscriminatedUnion.Generator.Tests.UnionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generator.Tests.EqualityFixtures
{
    public class EmployeeEqualityFixture : UnionEqualityFixture<Employee>
    {
        public override IEnumerable<Func<Employee>> SameValues
        {
            get
            {
                yield return () => Employee.NewEngineer("bob");
                yield return () => Employee.NewEngineer("jane");
                yield return () => Employee.NewManager("mary");
                yield return () => Employee.NewManager("charles");
                yield return () => Employee.NewTestEngineer("mary");
                yield return () => Employee.NewTestEngineer("paul");
            }
        }

        public override IEnumerable<(Employee, Employee)> DifferentValues
        {
            get
            {
                yield return (Employee.NewEngineer("bob"), Employee.NewEngineer("jane"));
                yield return (Employee.NewEngineer("jane"), Employee.NewManager("jane"));
                yield return (Employee.NewManager("mary"), Employee.NewEngineer("jane"));
                yield return (Employee.NewManager("charles"), Employee.NewTestEngineer("charles"));
                yield return (Employee.NewTestEngineer("mary"), Employee.NewEngineer("jane"));
                yield return (Employee.NewTestEngineer("paul"), Employee.NewEngineer("paul"));
            }
        }

        public override Employee AnonymousValue => Employee.NewEngineer("marcel");
    }
}
