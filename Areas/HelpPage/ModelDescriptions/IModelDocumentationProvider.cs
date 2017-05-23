using System;
using System.Reflection;

namespace Semprini.Mattia._5i.Calendar.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}