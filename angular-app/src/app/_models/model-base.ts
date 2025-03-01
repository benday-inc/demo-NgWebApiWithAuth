import { ApplicationConstants } from '../_common/application-constants';

export class ModelBase {
  id: string;
  etag: string;
  timestamp: Date;

  constructor() {
    this.id = ApplicationConstants.defaultId;
    this.etag = ApplicationConstants.defaultString;
    this.timestamp = ApplicationConstants.defaultDate;
  }
}
