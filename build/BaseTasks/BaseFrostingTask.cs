using Cake.Frosting;

public abstract class BaseFrostingTask<T> : FrostingTask<T> where T : BaseBuildContext
{
    protected string GetTaskName()
    {
        System.Attribute[] attrs = System.Attribute.GetCustomAttributes(this.GetType());

        foreach (System.Attribute attr in attrs)
        {
            if (attr is TaskNameAttribute)
            {
                TaskNameAttribute a = (TaskNameAttribute)attr;
                return a.Name;
            }
        }
        return "Undefine";
    }
}

