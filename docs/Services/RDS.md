# RDS on MountAws

TODO. In the meantime, you can just run `dir` within the `rds` directory and probably find your way around.
Remember that you can often `cd` into objects to find more related objects within. The hierarchy is structured in the following way:

```
-- rds
   |-- clusters
       |-- my-aurora-cluster
           |-- my-aurora-cluster-instance
           |-- my-aurora-cluster-replica-instance
   |-- instances
       |-- my-rds-instance
       |-- another-rds-instance
```