IEntity entity = Context.Target as IEntity;

MClient.Logger.Info($"Asset id {entity.Id.Value.ToString()} is being submitted for auto-approval");
await MClient.Assets.FinalLifeCycleManager.SubmitAsync(entity.Id.Value);

MClient.Logger.Info($"Asset id {entity.Id.Value.ToString()} is being auto-approved");
await MClient.Assets.FinalLifeCycleManager.ApproveAsync(entity.Id.Value);