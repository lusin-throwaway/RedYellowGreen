# RedYellowGreen

## Assumptions / interpretation

### Current situation

Currently workers walk around the factory and use their own judgement to determine the state of equipment, which they then mark with a magnet attached to the physical equipment.
The problem is that there's no high level overview of all the equipment as it's only possible to see a state of equipment by physically seeing which color magnet is attached to it. Same for setting the state.

Supervisors schedule orders, presumably in an analog way as well.

### The goal

The goal is to create a system for keeping track of equipment state indicators and orders.

#### States

The indication of state is still done by workers using their judgement by physically seeing the equipment when they are near it and then setting the state in the system.

#### Orders

Supervisors can schedule orders to equipment in the system and workers should be able to see order backlog for each equipment.

It is not clear from the requirements how it is indicated which order is being performed nor how it is marked as done. So I'm going to make a call and say that the current order is the least recent one (a queue) and a worker needs to mark it as done.

### Outcome

There should be 2 separate dashboards / views for different actors:
1. Supervisor view:
    - Requirements:
        - See a list of equipment and be able to see the state history of each
        - Schedule new orders to equipment
        - See all the scheduled orders and in what state is the equipment the order is assigned to, with live updates.
        - Done orders are out of scope of this dashboard and are removed from the view 
    - Implementation:
        - A table of equipment that indicates the equipment title and has a button 'see more details'
        - On clickin a specific equipments 'see more details' - a side panel opens that allows for scheduling new orders - a button 'schedule new order', and shows the history of states for that piece of equipment - a table with columns (state, timestamp)
        - A second table of all global orders, with columns (order number, assigned equipment title, equipment state).
        - Live updates on equipment state changes.
        - When an order is marked as done by a worker - it's removed from this view.
1. Worker view:
    - Requirements:
        - See a list of equipment with its current state, which is live updated
        - Be able to change individual equipment state
        - See what is the current scheduled order for each equipment, as well as the rest of the order backlog
        - Mark the current scheduled order as done. Since the state indicator of equipment is an observation and not nescesarily the actual state - it doesn't make sense to validate that an order can only be set as done if the equipment is in the Green state
    - Implementation:
        - A table of equipment with columns (title, state, current order, scheduled orders) 
        - state is a dropdown that will trigger a state change on different value selection
        - current scheduled order is the oldest scheduled order with a button to mark it complete
        - scheduled orders - a bullet list of scheduled orders

## Architecture / implementation breakdown

- postgres database

- .NET10 backend:
    - API - endpoints to fetch and modify data
        - Worker dashboard
            - GET worker-view/equipment - returns a list of equipment with fields
                - Id
                - Title
                - State
                - CurrentOrder (Id, Number)
                - Scheduled Orders (Id, Number, Timestamp)[]
            - PUT /equipment/{id}/state - set the state (R/Y/G)
            - POST /order/{id}/complete - mark order as completed
        - Supervisor dashboard
            - GET supervisor-view/equipment - equipment with fields:
                - Id
                - Title
                - State
            - GET /equipment/{id}/state-history returns a of equipment states:
                - State
                - Timestamp
            - POST equipment/{id}/orders - schedule order endpoint, no input

    - SignalR Hub that publishes updates on state changes
        - Updates on equipment state changes
        - updates on new orders
        - updates on completed orders

    - EF to communicate with the database
    - MassTransit for messaging
    - MSTest for testing, just because I have the most (recent) experience with it

- react/ts frontend:
    - Supervisor view
        - equipment table
            - side panel for specific equipment details
        - orders table
    - Worker view
        - equipment table