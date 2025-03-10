import { ApplicationConstants } from "../_common/application-constants";
import { UserClaim } from "./user-claim";

export class User {
    ownerId: string = ApplicationConstants.ownerIdForSystem;
    id: string = ApplicationConstants.defaultId;
    email: string = ApplicationConstants.defaultString;

    claims: UserClaim[] = [];
}
