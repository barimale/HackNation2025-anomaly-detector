import { useState, useEffect, useRef } from "react";
import UploadService from "../../services/FileUploadService";
import {AddDocumentIcon} from '@sanity/icons'
import { ActivityIcon } from '@sanity/icons'

interface ProgressInfo {
  fileName: string;
  percentage: number;
}

interface FilesUploadProps{
  OnMessageChanged: ()=> void;
  OnReset: () => void;
}

const FilesUpload: React.FC<FilesUploadProps> = (props : FilesUploadProps) => {
  const [workInProgres, setWorkInProgress] = useState<boolean>(false);
  const [selectedFiles, setSelectedFiles] = useState<FileList | null>(null);
  const [progressInfos, setProgressInfos] = useState<Array<ProgressInfo>>([]);
  const [message, setMessage] = useState<Array<string>>([]);
  const [reset, setReset] = useState(false);
  const progressInfosRef = useRef<any>(null);

      useEffect(() => {
      if(reset)
      {
        props.OnReset();
      }
  }, [reset]);

    useEffect(() => {
      if(message.length > 0)
      {
        props.OnMessageChanged();
      }
  }, [message]);

  useEffect(() => {
    setReset(false);
  }, []);

  const selectFiles = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSelectedFiles(event.target.files);
    setProgressInfos([]);
    setMessage([]);
    setReset(true);
  };

  const upload = (id: string, idx: number, file: File) => {
    let _progressInfos = [...progressInfosRef.current];

    return UploadService.upload(id, file, (event) => {
      _progressInfos[idx].percentage = Math.round(
        (100 * event.loaded) / event.total
      );
      setProgressInfos(_progressInfos);
    })
      .then(() => {
        setMessage((prevMessage) => [
          ...prevMessage,
          file.name + ": Wczytane z sukcesem!"
        ]);
      })
      .catch((err: any) => {
        _progressInfos[idx].percentage = 0;
        setProgressInfos(_progressInfos);
        console.log(JSON.stringify(err));
        let msg = file.name + ": Wczytywanie zakończone błędem!";
        if (err.response && err.response.data && err.response.data.message) {
          msg += " " + err.response.data.message;
        }

        setMessage((prevMessage) => [
          ...prevMessage,
          msg
        ]);
      });
  };

  const notify = (id: string) => {
    return UploadService.notify(id)
      .catch((err: any) => {
        console.log(JSON.stringify(err));
      });
  };

  const uploadFiles = () => {    
    setWorkInProgress(() => true);
    if (selectedFiles != null) {
      const files = Array.from(selectedFiles);

      let _progressInfos = files.map((file) => ({
        percentage: 0,
        fileName: file.name
      }));

      progressInfosRef.current = _progressInfos;
      const id = localStorage.getItem('sessionId');
      const uploadPromises = files.map((file, i) => upload(id!, i, file));
      Promise.all(uploadPromises).then(()=>{
        notify(id!);
      });
      setMessage([]);
    }
  };

  return (
    <div>
      <div className="row my-3">
        <div className="col-9">
          <label className="btn btn-default p-0">
            <label htmlFor="file-upload" style={{padding: '5px',paddingLeft: '10px', paddingRight: '10px', color: 'white', cursor: 'pointer',backgroundColor: 'transparent', border: '1px solid white', borderRadius: '3px'}}>
              <AddDocumentIcon /> Wczytaj plik
            </label>
            <input id="file-upload" style={{display: 'none'}} type="file" multiple onChange={selectFiles} />
          </label>
        </div>
        <div className="col-3">
          <button
            className="btn btn-success btn-sm"
            style={{
             backgroundColor: 'transparent', //#9184EE
             borderColor: 'white',
             color: 'white',
             fontSize: '16px',
             padding: '5px 10px'
            }}
            disabled={!selectedFiles || workInProgres}
            onClick={uploadFiles}
          >
            <ActivityIcon />
            Analizuj
          </button>
        </div>
      {progressInfos &&
        progressInfos.length > 0 &&(
          <div className="col-12" style={{ 
              overflowY: progressInfos.length > 2 ? 'scroll' : 'unset',
              height: progressInfos.length > 2 ? '100px' : 'unset',
              scrollbarColor: '#555 #1e1e1e',
            }}>{
            progressInfos.map((progressInfo: ProgressInfo, index: number) => (
              <div className="mb-2" key={index}>
                <span style={{fontSize: '12px'}}>{progressInfo.fileName}</span>
                <div className="progress">
                  <div
                    className="progress-bar progress-bar-info"
                    role="progressbar"
                    aria-valuenow={progressInfo.percentage}
                    aria-valuemin={0}
                    aria-valuemax={100}
                    style={{ width: progressInfo.percentage + "%"}}
                  >
                    {progressInfo.percentage}%
                  </div>
                </div>
              </div>
            ))}
        </div>)}
      </div>
      {message.length > 0 && (
        <>
          <p>Lista zakończonych wczytywań plików:</p>
          <div className="alert alert-secondary" role="alert"             style={{ 
                overflowY: message.length > 2 ? 'scroll' : 'unset',
                height: message.length > 2 ? '100px' : 'unset'
              }}>
            <ul>
              {message.map((item, i) => {
                return <li key={i} style={{fontSize: '12px'}}>{item}</li>;
              })}
            </ul>
          </div>
        </>
      )}
    </div>
  );
};

export default FilesUpload;
