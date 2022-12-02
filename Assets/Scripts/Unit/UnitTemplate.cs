
public class UnitTemplate
{
    public UnitProperty property;
    TagController.UnitFunction runTimeFunction;

    public UnitTemplate(UnitProperty property, TagController.UnitFunction runTimeFunction)
    {
        this.property = property;
        this.runTimeFunction = runTimeFunction;
    }
}
