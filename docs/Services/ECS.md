# ECS on MountAws

TODO. In the meantime, you can just run `dir` within the `ecs` directory and probably find your way around.
Remember that you can often `cd` into objects to find more related objects within. The hierarchy looks like this:

```
-- ecs
   |-- clusters
       |-- my-cluster
           |-- container-instances
               |-- 018fa2a62f0549abb15f62234f5d4bc8 #ECS Container Instance ID
               |-- 771f6bee9cb34f15819c11c96af7db3c
           |-- services
               |-- my-service
                   |-- db3868af023e43b0802ec414efc5efd0 #Task ID
   |-- task-families
       |-- my-task-family
           |-- 1 #task definition revision
```