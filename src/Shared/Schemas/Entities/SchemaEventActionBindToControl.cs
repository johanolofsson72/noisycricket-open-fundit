namespace Shared.Schemas.Entities;

public class SchemaEventActionBindToControl(
    int identifier, 
    int bindToControlId) 
    : SchemaEventAction(
        identifier, 
        bindToControlId);