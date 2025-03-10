import { ApplicationConstants } from "../_common/application-constants";

export class User {
    ownerId: string = ApplicationConstants.ownerIdForSystem;
    id: string = ApplicationConstants.defaultId;
    email: string = ApplicationConstants.defaultString;
}
