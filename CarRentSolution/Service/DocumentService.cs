using Aspose.Words;
using CarRentSolution.Entity;

namespace CarRentSolution.Service;

public abstract class DocumentService
{
    private static string _path = @"..//..//..//Documents//";

    public void SaveFile(Employee employee, Rent rent)
    {
        Document document = new Document(Path.Combine(_path, "template.docx"));
    }
}