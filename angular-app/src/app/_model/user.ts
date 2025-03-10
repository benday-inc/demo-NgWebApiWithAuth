import { ApplicationConstants } from "../_common/application-constants";

export class User {
    ownerId: string = ApplicationConstants.ownerIdForSystem;
    id: string = ApplicationConstants.defaultId;
    name: string = ApplicationConstants.defaultString;
    email: string = ApplicationConstants.defaultString;
    password: string = ApplicationConstants.defaultString;
    status: string = ApplicationConstants.defaultStatus;
}
