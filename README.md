# Roadside Assistance

### Solution File
```
RoadSideAssistance.sln
```

### Build and Test Instructions from root directory
```
dotnet build
dotnet test
```

### Main Test
```csharp
[Fact]
public void ShouldReturnSortedSetOfAssistants()
{
    var assistantsList = _service.FindNearestAssistants(_laLocation, 10).ToList(); // From LA
    assistantsList[0].AssistantId.Should().Be(_providers[3].ServiceProviderId); // Portland
    assistantsList[1].AssistantId.Should().Be(_providers[0].ServiceProviderId); // Seattle
    assistantsList[2].AssistantId.Should().Be(_providers[1].ServiceProviderId); // Vancouver
    assistantsList[3].AssistantId.Should().Be(_providers[2].ServiceProviderId); // Chicago
}
```

### Service Implementation

```
RoadSideAssistance\RoadSideAssistance.Application\Services\RoadSideAssistanceService.cs
```

### In-memory Repository

```
RoadSideAssistance\RoadsideAssistance.Persistence\Repositories\RoadsideServiceProviderRepository.cs
```

### Unit Test
```
RoadSideAssistance\RoadsideAssistance.Application.UnitTests\Services\RoadSideAssistanceServiceTests.cs
```

### RFC Request for Change to RoadsideAssistanceService interface
- `Assistant` entity is used as an input paramenter and as a return type.

  `void updateAssistantLocation(Assistant assistant, Geolocation assistantLocation);`
  `SortedSet<Assistant> findNearestAssistants(Geolocation geolocation, int limit);`

   When return a sorted set the Assistant entity must have a field such as `DistanceFromCustomer` that can be used to sort the collection whereas when used as an input parameter the DistanceFromCustomer field is not used. We need a separate type for input and return value
- `SortedSet<Assistant>` is not the best return type for method `findNearestAssistants(Geolocation geolocation, int limit);`

  SortedSet is a concrete implementation and does not give the opportunity to change the implemenation in the future.

  Another thought is to let the consumers of the method sort the nearest Assistant list.

- `Optional<Assistant> reserveAssistant(Customer customer, Geolocation customerLocation);`
  This method should instead attempt to reserve the specified assistant to the customer instead. 


### Implementation Considerations
- Dependency Inversion
- Testability
- Repository Pattern
- Thread Safety
- Loose Coupling
- Clean Code Architecture
- Persistance Ignorance
- Separation of Concerns
- Single Responsibility
- Guard clauses


### Assumptions
- 
  ```csharp
  Optional<Assistant> reserveAssistant(Customer customer, Geolocation customerLocation);
  ```
  Method reserves the closes Assistant for the customer

### Compromises
- Used `Nullable` types instead of Java `Optional<>`
- Not used `async` implementation
- Testing is limited to only the core implementations
- NetTopologySuite measures distance in Degrees instead of miles by default

### Limitations
- Not Thread Safe
  
  Multiple consumers of the service can attempt to reserve the same provider when invoked concurrently. An optimistic locking / concurrency mechanism with row versioning would be a good option here

  A queuing mechanism can also be implemented to handle one request at a time such that assignments for a specific Assistant is queued


### Missed Opportunities
- Logging