import "bootstrap/dist/css/bootstrap.min.css";
import "./App.css";
import Modal from "../Organisms/Modal";
import {useEffect, useState }from "react"
import FilesUpload from "../Organisms/FilesUpload";
import { HubConnectionBuilder, LogLevel, IHttpConnectionOptions, HttpTransportType } from '@microsoft/signalr';
import OverallResult from "../../types/overall-result";
import { Guid } from "guid-typescript";
import { Tooltip } from 'react-tooltip'
import PieChartWithCustomizedLabel, { Data } from '../Molecules/TooltipContent';
import LoadingCircle from "../Atoms/LoadingCircle";
import axios from "axios";
import Ranking from "../../types/ranking";
import MessageBox from "../Molecules/MessageBox";
import {RefreshIcon} from '@sanity/icons'
import {OlistIcon} from '@sanity/icons'
import {BarChartIcon} from '@sanity/icons'

const App: React.FC = () => {
  const [message, setMessage] = useState<Array<string>>([]);
  const [data, setData] = useState<Array<Data>>([]);
  const [ranks, setRanks] = useState<Array<Ranking>>([]);
  const [reset, setReset] = useState<boolean>(false);
  const [show, setShow] = useState<boolean>(false);
  const [systemResult, setSystemResult] = useState<string>("");
  const [anomalyNotDetected, setAnomalyNotDetected] = useState<boolean | null>(null);
  const options: IHttpConnectionOptions = {
    withCredentials: false,
    accessTokenFactory: () => Guid.create().toString(),
    skipNegotiation: true,
    transport: HttpTransportType.WebSockets
  };
    const [showBox, setShowBox] = useState(false);

  const handleDelete = () => {
    console.log("Item deleted!");
    setShowBox(false);
  };

    useEffect(()=>{
      const guid = Guid.create().toString();
      localStorage.setItem('sessionId', guid);
  }, []);

  const connection = new HubConnectionBuilder()
    .withUrl(process.env.REACT_APP_API_URL! + process.env.REACT_APP_API_SIGNALR_ENDPOINT!, options)
    .configureLogging(LogLevel.Information)
    .withAutomaticReconnect()
    .build();

  connection.on('OnAnomalyDetectedAsync', (id: OverallResult) => {
    setSystemResult(JSON.stringify(id));
    setAnomalyNotDetected(() => false);
  });

    connection.on('onChangeAsync', (agentName: string, value: string, number: number, sessionId: string) => {
      if(sessionId === localStorage.getItem('sessionId'))
      {
    setMessage((prevMessage) => [
          ...prevMessage,
          agentName + ": " + (value === 'True' ? "anomalia wykryta" : 'brak anomalii')
        ]);

    if(value === 'True')
    {
      setData((prevData) => [
          ...prevData,
          {
            name: agentName,
            value: number,
            color: value === 'True' ? 'red' : 'green'
          }
        ]);
    }else{
      setData((prevData) => [
          {
            name: agentName,
            value: number,
            color: value === 'True' ? 'red' : 'green'
          },
          ...prevData
        ]);
    }
  }
  });

  connection.on('OnAnomalyNotDetectedAsync', (id: OverallResult) => {
    setAnomalyNotDetected(() => true);
  });

  useEffect(() => {
    const startSystem = async () => {
      try {
        connection.serverTimeoutInMilliseconds = 600000; // 60 seconds
        connection.keepAliveIntervalInMilliseconds = 15000;
        await connection.start()
          .then(() => {
            console.log('connection started');
          }).catch((error: any) => {
            console.log('connection failed: ' + error);
          });
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    };
    
    startSystem();
    return () => {
      console.log("Component with signalR unmount.");
      connection.stop();
    };
  }, []);

  useEffect(()=>{
    setMessage([]);
    setData([]);
    if(reset)
    {
      setShow(false);
      if(connection.state === 'Disconnected')
      {
        connection.serverTimeoutInMilliseconds = 600000; // 60 seconds
        connection.keepAliveIntervalInMilliseconds = 15000;
        connection.start()
          .then(() => {
            console.log('connection started');
          }).catch((error: any) => {
            console.log('connection failed: ' + error);
          });
      }
      
    }
  }, [reset]);

    const refreshPage = () => {
      window.location.reload();
  };

  const showRank = async () => {
      await axios.get<Ranking[]>(process.env.REACT_APP_API_URL + '/api/data/ranking', {
          timeout: 15000,
          headers: {
              'Accept': 'application/json'
          }
      }).then((response)=>{
        setRanks(response.data);
        setShowBox(true);
      })
      .catch(ex=> {
        console.log(ex);
      });

  };

  return (
    <>
    <Modal isOpen={true} onClose={()=> {}}>
      <div className="container" style={{ height:'640px', backgroundColor: '#212529'}}>
        <div className="my-3">
          <div style={{display:' flex'}}>
            <h2 style={{color: '#325965'}}>Hack</h2>
            <h2 style={{color: '#E7003A'}}>Nation 2025</h2>
          </div>
          <h5>Wyszukiwarka anomalii</h5>
        </div>
        <div style={{border: '5px solid #E7003A', height: '5px'}}></div>
        <FilesUpload 
          OnMessageChanged={() => {setShow((true))}}
          OnReset={() => {setReset(true)}}
        />
        {data.length > 0 &&
          <Tooltip anchorSelect=".my-anchor-element" place="top" opacity={1} style={{zIndex: 500}}>
            Wykres kołowy głosowań agentów:
            <PieChartWithCustomizedLabel isAnimationActive={false} data={data}/>
          </Tooltip>
        }
        { message.length > 0 ?(
        <>
          <p>Lista zakończonych wyszukiwań agentów:</p>
          <div 
            className="alert alert-secondary" 
            style={{ 
              overflowY: 'scroll',
              height: '100px'
            }}
            role="alert">
            <ul>
              {message.flatMap((item, i) => {
                return <li key={i} style={{fontSize: '12px'}}>{item}</li>;
              })}
            </ul>
          </div>
        </>
        ):(
          (show) && 
            <div style={{
              display: 'flex',
              justifyContent: 'center'
            }}>
              <LoadingCircle/>
            </div>
          )
        }
                <p style={{
          textDecoration: 'none',
          color: anomalyNotDetected != null ? (anomalyNotDetected ? 'green' : 'red') : 'white'
        }}>{anomalyNotDetected != null && show? (anomalyNotDetected ? 
          'Brak anomalii!' : 
          'Anomalia wykryta przez system.') : 
          show ? '' : ''}
        </p>
      </div>
      
      <div style={{height: '1px', border: '5px solid white'}}></div>
      <div style={{display: 'flex', justifyContent: 'space-between', alignItems: 'center'}}>
        <button
          className="btn btn-sm"
          style={{
            backgroundColor: 'transparent',
            color: '#007BFF'
          }}
          onClick={refreshPage}
          >
            <RefreshIcon />
          Odśwież stronę
        </button>
                <button
          className="btn btn-sm"
          style={{
            backgroundColor: 'transparent',
            color: '#007BFF'
          }}
          onClick={showRank}
          >
          <OlistIcon/>
           Zobacz ranking
        </button>
          <a className="my-anchor-element" style={{cursor: 'pointer', fontSize: '14px'}}><BarChartIcon/> Szczegóły głosowań</a>
        </div>
    </Modal>
     <MessageBox
        title="Ranking"
        message={<ol style={{
              justifyContent: 'flex-start',
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'flex-start'
        }}>{ranks
          .flatMap((p: Ranking) => 
            (<li>{p.algorithmId}: <b>{p.score}</b></li>))}
        </ol>}
        show={showBox}
        onConfirm={handleDelete}
        confirmText="OK"
        cancelText="No"
      />
    </>
  );
}

export default App;
