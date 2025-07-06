using System;

namespace backendLogic.src.searchEngine.models;

public class ProjectBuilder
{
    private ProjectTypeEnum ProjectType { get; set; }
    public ProjectType Project { get; private set; }
    public ProjectBuilder(ProjectTypeEnum projectType)
    {
        ProjectType = projectType;
        Project = IntializedProject();

    }

    private ProjectType IntializedProject()
    {
        switch (ProjectType)
        {
            case ProjectTypeEnum.Node:
                return new Node();
            case ProjectTypeEnum.PDF:
                return new PDF();
            default:
                throw new NotImplementedException($"Project type {ProjectType} is not implemented.");
        }
    }


}
